using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Fields.Commands
{
    public record CreateFieldCommand(
        string FieldName,
        string? Description,
        double Latitude,
        double Longitude,
        string? ImageUrl,
        string? ThumbnailUrl,
        Guid OwnerId
    ) : IRequest<Result<Guid>>;
}
