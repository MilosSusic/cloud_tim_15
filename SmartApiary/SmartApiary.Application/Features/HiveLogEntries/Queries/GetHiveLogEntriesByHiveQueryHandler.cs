using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.HiveLogEntries.Queries
{
    public class GetHiveLogEntriesByHiveQueryHandler
        : IRequestHandler<GetHiveLogEntriesByHiveQuery, Result<PagedResult<HiveLogEntryDto>>>
    {
        private readonly IHiveLogEntryRepository _logRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetHiveLogEntriesByHiveQueryHandler(
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

        public async Task<Result<PagedResult<HiveLogEntryDto>>> Handle(
            GetHiveLogEntriesByHiveQuery request,
            CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result<PagedResult<HiveLogEntryDto>>.Failure("Invalid user id.");

            var hive = await _hiveRepository.GetByIdAsync(request.HiveId, cancellationToken);
            if (hive is null)
                return Result<PagedResult<HiveLogEntryDto>>.Failure("Hive not found.");

            var apiary = await _apiaryRepository.GetByIdAsync(hive.ApiaryId, cancellationToken);
            if (apiary is null || apiary.OwnerId != userId)
                return Result<PagedResult<HiveLogEntryDto>>.Failure("You do not have access to this hive.");

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            var (entries, totalCount) = await _logRepository.GetPagedByHiveIdAsync(
                request.HiveId, page, pageSize, cancellationToken);

            var dtos = entries
                .Select(e => new HiveLogEntryDto(
                    e.Id,
                    e.HiveId,
                    e.ReviewDate,
                    e.ReviewType,
                    e.BottomBoardColor,
                    e.HoneyFrames,
                    e.HoneyAmountKg,
                    e.BroodFrames,
                    e.QueenPresent,
                    e.Note))
                .ToList();

            var paged = new PagedResult<HiveLogEntryDto>(dtos, totalCount, page, pageSize);
            return Result<PagedResult<HiveLogEntryDto>>.Success(paged);
        }
    }
}
