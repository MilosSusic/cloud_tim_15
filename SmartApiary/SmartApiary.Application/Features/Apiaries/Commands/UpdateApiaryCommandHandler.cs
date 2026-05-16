using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Apiaries.Commands
{
    public class UpdateApiaryCommandHandler : IRequestHandler<UpdateApiaryCommand, Result>
    {
        private readonly IApiaryRepository _apiaryRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateApiaryCommandHandler(
            IApiaryRepository apiaryRepository,
            ICurrentUserService currentUserService)
        {
            _apiaryRepository = apiaryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(UpdateApiaryCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result.Failure("Invalid user id.");

            var apiary = await _apiaryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (apiary is null)
                return Result.Failure("Apiary not found.");

            if (apiary.OwnerId != userId.Value)
                return Result.Failure("You do not have access to this apiary.");

            try
            {
                apiary.UpdateDetails(
                    request.Name,
                    request.Description,
                    request.Latitude,
                    request.Longitude,
                    request.ImageUrl,
                    request.ThumbnailUrl);
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }

            _apiaryRepository.Update(apiary);
            await _apiaryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
