using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class Crop
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public CropType CropName { get; set; }
        public DateTime FloweringPeriodStart { get; set; }
        public DateTime FloweringPeriodEnd { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}
