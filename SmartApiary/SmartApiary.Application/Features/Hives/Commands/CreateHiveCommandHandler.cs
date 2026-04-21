using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Hives.Commands
{
    public class CreateHiveCommandHandler : IRequestHandler<CreateHiveCommand, Result<Guid>>
    {
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateHiveCommandHandler(
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<Guid>> Handle(CreateHiveCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result<Guid>.Failure("Invalid user id.");

            var apiary = await _apiaryRepository.GetByIdAsync(request.ApiaryId, cancellationToken);
            if (apiary is null)
                return Result<Guid>.Failure("Apiary not found.");

            if (apiary.OwnerId != userId)
                return Result<Guid>.Failure("You do not have access to this apiary.");

            var hive = new Hive
            {
                Id = Guid.NewGuid(),
                ApiaryId = request.ApiaryId,
                Label = request.Label,
                Type = request.Type,
                Color = request.Color,
                QueenAge = request.QueenAge,
                Note = request.Note,
                IsDeleted = false
            };

            await _hiveRepository.AddAsync(hive, cancellationToken);
            await _hiveRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(hive.Id);
        }
    }
}
