using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Hives.Queries
{
    public record GetHivesByApiaryQuery(Guid ApiaryId) : IRequest<Result<List<HiveDto>>>;
}
