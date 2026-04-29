using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class SpraySchedule
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public DateTime StartDateTime { get; set; }
        public int DurationHours { get; set; }
        public string? PesticideType { get; set; }
        public SprayStatus Status { get; set; } = SprayStatus.Scheduled;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
