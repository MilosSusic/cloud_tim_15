using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Apiaries.Commands
{
    public record UpdateApiaryCommand(
        Guid Id,
        string Name,
        string? Description,
        double Latitude,
        double Longitude,
        string? ImageUrl,
        string? ThumbnailUrl
    ) : IRequest<Result>;
}
