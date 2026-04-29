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
        public CropType CropName { get; set; } = CropType.None;
        public string? Description { get; set; }
        public string? Note { get; set; }
        public string Color { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
