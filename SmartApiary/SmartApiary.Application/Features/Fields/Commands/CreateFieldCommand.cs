using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
