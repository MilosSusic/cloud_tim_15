using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Hives.Queries
{
    public class GetHivesByApiaryQueryHandler : IRequestHandler<GetHivesByApiaryQuery, Result<List<HiveDto>>>
    {
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetHivesByApiaryQueryHandler(
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<List<HiveDto>>> Handle(GetHivesByApiaryQuery request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result<List<HiveDto>>.Failure("Invalid user id.");

            var apiary = await _apiaryRepository.GetByIdAsync(request.ApiaryId, cancellationToken);
            if (apiary is null)
                return Result<List<HiveDto>>.Failure("Apiary not found.");

            if (apiary.OwnerId != userId)
                return Result<List<HiveDto>>.Failure("You do not have access to this apiary.");

            var hives = await _hiveRepository.GetByApiaryIdAsync(request.ApiaryId, cancellationToken);

            var result = hives
                .Select(h => new HiveDto(h.Id, h.ApiaryId, h.Label, h.Type, h.Color, h.QueenAge, h.Note))
                .ToList();

            return Result<List<HiveDto>>.Success(result);
        }
    }
}
