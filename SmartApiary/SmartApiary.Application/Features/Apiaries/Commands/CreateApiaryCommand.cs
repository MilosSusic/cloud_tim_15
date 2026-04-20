using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Apiaries.Commands
{
    public record CreateApiaryCommand(
        string Name,
        string? Description,
        double Latitude,
        double Longitude,
        string? ImageUrl,
        string? ThumbnailUrl,
        Guid OwnerId
    ) : IRequest<Result<Guid>>;
}
