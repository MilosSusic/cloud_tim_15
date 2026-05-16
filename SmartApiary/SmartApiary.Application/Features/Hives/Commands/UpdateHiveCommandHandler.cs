using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Hives.Commands
{
    public class UpdateHiveCommandHandler : IRequestHandler<UpdateHiveCommand, Result>
    {
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateHiveCommandHandler(
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            ICurrentUserService currentUserService)
        {
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateHiveCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result.Failure("Invalid user id.");

            var hive = await _hiveRepository.GetByIdAsync(request.Id, cancellationToken);
            if (hive is null)
                return Result.Failure("Hive not found.");

            var apiary = await _apiaryRepository.GetByIdAsync(hive.ApiaryId, cancellationToken);
            if (apiary is null || apiary.OwnerId != userId.Value)
                return Result.Failure("You do not have access to this hive.");

            try
            {
                hive.UpdateDetails(
                    request.Label,
                    request.Type,
                    request.Color,
                    request.QueenAge,
                    request.Note);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }

            _hiveRepository.Update(hive);
            await _hiveRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
