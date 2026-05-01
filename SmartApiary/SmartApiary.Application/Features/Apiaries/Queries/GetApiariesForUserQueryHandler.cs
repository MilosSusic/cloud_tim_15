using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Apiaries.Queries
{
    public class GetApiariesForUserQueryHandler : IRequestHandler<GetApiariesForUserQuery, Result<List<ApiaryListItemDto>>>
    {
        private readonly IApiaryRepository _apiaryRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetApiariesForUserQueryHandler(IApiaryRepository apiaryRepository, ICurrentUserService currentUserService)
        {
            _apiaryRepository = apiaryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<ApiaryListItemDto>>> Handle(GetApiariesForUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId is null)
                return Result<List<ApiaryListItemDto>>.Failure("Invalid user id.");

            var apiaries = await _apiaryRepository.GetByOwnerIdAsync(userId.Value, cancellationToken);
            var dtos = apiaries.Select(a => new ApiaryListItemDto
            {
                Id = a.Id,
                Name = a.Name,
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                ImageUrl = a.ImageUrl,
                ThumbnailUrl = a.ThumbnailUrl
            }).ToList();
            return Result<List<ApiaryListItemDto>>.Success(dtos);
        }
    }
}
