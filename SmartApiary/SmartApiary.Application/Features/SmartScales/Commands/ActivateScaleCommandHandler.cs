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

            if (scale.Status == SmartScaleStatus.Paired)
            {
                return Result<string>.Success(scale.AccessToken);
            }

            if (request.HardwareId == Guid.Empty)
            {
                return Result<string>.Failure("HARDWARE_ID_REQUIRED");
            }

            string generatedToken = GenerateSecureToken();

            var activationResult = scale.Activate(request.HardwareId, generatedToken);
            if (activationResult.IsFailure)
                return Result<string>.Failure($"Domain error: {activationResult.Error}");

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
