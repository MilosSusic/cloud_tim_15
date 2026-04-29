using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
