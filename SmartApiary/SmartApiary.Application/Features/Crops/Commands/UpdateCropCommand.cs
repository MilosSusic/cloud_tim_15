using MediatR;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Crops.Commands
{
    public record UpdateCropCommand(
        Guid Id,
        CropType CropName,
        string? Description
    ) : IRequest<Result>;
}
