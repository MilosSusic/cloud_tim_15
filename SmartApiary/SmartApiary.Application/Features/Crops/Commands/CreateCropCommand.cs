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
    public record CreateCropCommand(
        Guid FieldId,
        CropType CropName,
        string? Description,
        DateTime FloweringPeriodStart,
        DateTime FloweringPeriodEnd
    ) : IRequest<Result<Guid>>;
}
