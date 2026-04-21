using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.HiveLogEntries.Queries
{
    public record GetHiveLogEntriesByHiveQuery(Guid HiveId, int Page = 1, int PageSize = 10)
        : IRequest<Result<PagedResult<HiveLogEntryDto>>>;
}
