using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class FieldLogEntry
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public DateTime ReviewDate { get; set; }
        public Guid CropId { get; set; }
        public string ReviewType { get; set; } = string.Empty;
        public string WeatherConditions { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string BottomBoardColor { get; set; } = string.Empty;
    }
}
