using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.HiveLogEntries.Commands
{
    public class CreateHiveLogEntryCommandHandler : IRequestHandler<CreateHiveLogEntryCommand, Result<Guid>>
    {
        private readonly IHiveLogEntryRepository _logRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateHiveLogEntryCommandHandler(
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

        public async Task<Result<Guid>> Handle(CreateHiveLogEntryCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result<Guid>.Failure("Invalid user id.");

            var hive = await _hiveRepository.GetByIdAsync(request.HiveId, cancellationToken);
            if (hive is null)
                return Result<Guid>.Failure("Hive not found.");

            var apiary = await _apiaryRepository.GetByIdAsync(hive.ApiaryId, cancellationToken);
            if (apiary is null || apiary.OwnerId != userId.Value)
                return Result<Guid>.Failure("You do not have access to this hive.");

            HiveLogEntry entry;
            try
            {
                entry = HiveLogEntry.Create(
                    request.HiveId,
                    request.ReviewDate,
                    request.ReviewType,
                    request.BottomBoardColor,
                    request.HoneyFrames,
                    request.HoneyAmountKg,
                    request.BroodFrames,
                    request.QueenPresent,
                    request.Note);
            }
            catch (ArgumentException ex)
            {
                return Result<Guid>.Failure(ex.Message);
            }

            await _logRepository.AddAsync(entry, cancellationToken);
            await _logRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(entry.Id);
        }
    }
}
