using MediatR;
using SmartApiary.Application.Features.Apiaries.Queries;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SmartApiary.Application.Features.Apiaries.Queries
{
    public class GetApiariesForUserQueryHandler : IRequestHandler<GetApiariesForUserQuery, Result<List<ApiaryListItemDto>>>
    {
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetApiariesForUserQueryHandler(IApiaryRepository apiaryRepository, IHttpContextAccessor httpContextAccessor)
        {
            _apiaryRepository = apiaryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<ApiaryListItemDto>>> Handle(GetApiariesForUserQuery request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result<List<ApiaryListItemDto>>.Failure("Invalid user id.");

            var apiaries = await _apiaryRepository.GetByOwnerIdAsync(userId, cancellationToken);
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
