using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Apiaries.Commands
{
    public class CreateApiaryCommandHandler : IRequestHandler<CreateApiaryCommand, Result<Guid>>
    {
        private readonly IApiaryRepository _apiaryRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateApiaryCommandHandler(IApiaryRepository apiaryRepository, ICurrentUserService currentUserService)
        {
            _apiaryRepository = apiaryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateApiaryCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result<Guid>.Failure("Invalid user id.");

            var apiaryResult = Apiary.Create(
                userId.Value,
                request.Name,
                request.Description,
                request.Latitude,
                request.Longitude,
                request.ImageUrl,
                request.ThumbnailUrl);

            if (apiaryResult.IsFailure)
                return Result<Guid>.Failure(apiaryResult.Error!);

            var apiary = apiaryResult.Value!;

            await _apiaryRepository.AddAsync(apiary, cancellationToken);
            await _apiaryRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(apiary.Id);
        }
    }
}
