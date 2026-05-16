using SmartApiary.Domain.Enums;

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
        public string? WeatherWarning { get; set; }

        public static SpraySchedule Create(Guid fieldId, DateTime startDateTime, int durationHours, string? pesticideType)
        {
            return new SpraySchedule
            {
                Id = Guid.NewGuid(),
                FieldId = fieldId,
                StartDateTime = startDateTime,
                DurationHours = durationHours,
                PesticideType = pesticideType,
                Status = SprayStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Reschedule(DateTime newStartDateTime)
        {
            StartDateTime = newStartDateTime;
            Status = SprayStatus.Rescheduled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(SprayStatus newStatus)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(DateTime newStartDateTime, SprayStatus newStatus, int newDurationHours, string? newPesticideType)
        {
            StartDateTime = newStartDateTime;
            Status = newStatus;
            DurationHours = newDurationHours;
            PesticideType = newPesticideType;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}