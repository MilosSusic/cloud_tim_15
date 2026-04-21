using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Hives.Commands
{
    public record UpdateHiveCommand(
        Guid Id,
        string Label,
        string Type,
        string Color,
        int QueenAge,
        string? Note
    ) : IRequest<Result>;
}
