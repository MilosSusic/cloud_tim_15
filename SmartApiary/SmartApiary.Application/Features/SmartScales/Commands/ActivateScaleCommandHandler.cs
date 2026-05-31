using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public class ActivateScaleCommandHandler : IRequestHandler<ActivateScaleCommand, Result<string>>
    {
        private readonly ISmartScaleRepository _smartScaleRepository;

        public ActivateScaleCommandHandler(ISmartScaleRepository smartScaleRepository)
        {
            _smartScaleRepository = smartScaleRepository;
        }

        public async Task<Result<string>> Handle(ActivateScaleCommand request, CancellationToken cancellationToken)
        {
            var scale = await _smartScaleRepository.GetBySerialNumberAsync(request.SerialNumber, cancellationToken);
            if (scale is null)
            {
                return Result<string>.Failure("A device with this serial number has not been registered by a beekeeper.");
            }

            // SCENARIO 2: Vaga je već uparena -> Odmah vrati njen postojeći token!
            if (scale.Status == SmartScaleStatus.Paired)
            {
                // Pretpostavka je da tvoj entitet čuva AccessToken. Ako se zove drugačije, prilagodi naziv.
                return Result<string>.Success(scale.AccessToken);
            }

            // SCENARIO 1 (Prvi korak): Vaga postoji ali nije uparena, a simulator je poslao Guid.Empty
            if (request.HardwareId == Guid.Empty)
            {
                // Šaljemo jasan signal simulatoru da je vaga slobodna za uparivanje i da mu treba Hardware ID
                return Result<string>.Failure("HARDWARE_ID_REQUIRED");
            }

            // SCENARIO 1 (Drugi korak): Vaga nije uparena, a simulator je poslao izgenerisani HardwareId
            string generatedToken = GenerateSecureToken();

            try
            {
                scale.Activate(request.HardwareId, generatedToken);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Domain error: {ex.Message}");
            }

            _smartScaleRepository.Update(scale);
            await _smartScaleRepository.SaveChangesAsync(cancellationToken);

            return Result<string>.Success(generatedToken);
        }

        private static string GenerateSecureToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "");
        }
    }
}
