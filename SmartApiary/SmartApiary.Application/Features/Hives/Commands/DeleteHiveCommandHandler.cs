using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Hives.Commands
{
    public class DeleteHiveCommandHandler : IRequestHandler<DeleteHiveCommand, Result>
    {
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteHiveCommandHandler(
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(DeleteHiveCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result.Failure("Invalid user id.");

            var hive = await _hiveRepository.GetByIdAsync(request.Id, cancellationToken);
            if (hive is null)
                return Result.Failure("Hive not found.");

            var apiary = await _apiaryRepository.GetByIdAsync(hive.ApiaryId, cancellationToken);
            if (apiary is null || apiary.OwnerId != userId)
                return Result.Failure("You do not have access to this hive.");

            hive.IsDeleted = true;

            _hiveRepository.Update(hive);
            await _hiveRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
