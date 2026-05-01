using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.HiveLogEntries.Commands
{
    public class DeleteHiveLogEntryCommandHandler : IRequestHandler<DeleteHiveLogEntryCommand, Result>
    {
        private readonly IHiveLogEntryRepository _logRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly ICurrentUserService _currentUserService;

        public DeleteHiveLogEntryCommandHandler(
            IHiveLogEntryRepository logRepository,
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            ICurrentUserService currentUserService)
        {
            _logRepository = logRepository;
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(DeleteHiveLogEntryCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result.Failure("Invalid user id.");

            var entry = await _logRepository.GetByIdAsync(request.Id, cancellationToken);
            if (entry is null)
                return Result.Failure("Log entry not found.");

            var hive = await _hiveRepository.GetByIdAsync(entry.HiveId, cancellationToken);
            if (hive is null)
                return Result.Failure("Hive not found.");

            var apiary = await _apiaryRepository.GetByIdAsync(hive.ApiaryId, cancellationToken);
            if (apiary is null || apiary.OwnerId != userId.Value)
                return Result.Failure("You do not have access to this log entry.");

            _logRepository.Delete(entry);
            await _logRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
