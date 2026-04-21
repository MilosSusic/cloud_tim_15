using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.HiveLogEntries.Commands
{
    public class DeleteHiveLogEntryCommandHandler : IRequestHandler<DeleteHiveLogEntryCommand, Result>
    {
        private readonly IHiveLogEntryRepository _logRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteHiveLogEntryCommandHandler(
            IHiveLogEntryRepository logRepository,
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _logRepository = logRepository;
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(DeleteHiveLogEntryCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result.Failure("Invalid user id.");

            var entry = await _logRepository.GetByIdAsync(request.Id, cancellationToken);
            if (entry is null)
                return Result.Failure("Log entry not found.");

            var hive = await _hiveRepository.GetByIdAsync(entry.HiveId, cancellationToken);
            if (hive is null)
                return Result.Failure("Hive not found.");

            var apiary = await _apiaryRepository.GetByIdAsync(hive.ApiaryId, cancellationToken);
            if (apiary is null || apiary.OwnerId != userId)
                return Result.Failure("You do not have access to this log entry.");

            _logRepository.Delete(entry);
            await _logRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
