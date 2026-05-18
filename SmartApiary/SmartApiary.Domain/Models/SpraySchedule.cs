using SmartApiary.Domain.Enums;

namespace SmartApiary.Domain.Models
{
    public class SpraySchedule
    {
        public Guid Id { get; private set; }
        public Guid FieldId { get; private set; }
        public DateTime StartDateTime { get; private set; }
        public int DurationHours { get; private set; }
        public string? PesticideType { get; private set; }
        public SprayStatus Status { get; private set; } = SprayStatus.Scheduled;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public string? WeatherWarning { get; private set; }

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

        public void SetWeatherWarning(string? weatherWarning)
        {
            WeatherWarning = weatherWarning;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}