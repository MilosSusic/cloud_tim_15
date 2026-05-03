using MediatR;
using SmartApiary.Application.Features.Fields.Queries;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Crops.Queries
{
    public record GetCropsForFieldIdQuery(Guid FieldId) : IRequest<Result<List<CropListItemDto>>>;

    public class CropListItemDto
    {
        public Guid Id { get; set; }
        public CropType CropName { get; set; } = CropType.None;
        public string? Description { get; set; }
        public DateTime FloweringPeriodStart { get; set; }
        public DateTime FloweringPeriodEnd { get; set; }
        public bool IsDeleted { get; set; }

    }
}
