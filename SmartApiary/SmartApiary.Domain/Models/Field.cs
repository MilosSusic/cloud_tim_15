using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class Field
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid OwnerId { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<Crop> Crops { get; set; } = new();
        public List<FieldLogEntry> LogEntries { get; set; } = new();
        public List<SpraySchedule> SpraySchedules { get; set; } = new();
    }
}
