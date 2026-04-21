using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Hives.Commands
{
    public record CreateHiveCommand(
        Guid ApiaryId,
        string Label,
        string Type,
        string Color,
        int QueenAge,
        string? Note
    ) : IRequest<Result<Guid>>;
}
