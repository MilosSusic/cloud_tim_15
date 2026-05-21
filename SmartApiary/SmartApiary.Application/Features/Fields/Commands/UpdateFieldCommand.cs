using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Fields.Commands
{
    public record UpdateFieldCommand(
        Guid Id,
        string FieldName,
        string? Description,
        double Latitude,
        double Longitude,
        string? ImageUrl,
        string? ThumbnailUrl
    ) : IRequest<Result>;
}
