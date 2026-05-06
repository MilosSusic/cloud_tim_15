namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class CreateSprayScheduleResult
    {
        public Guid SprayScheduleId { get; set; }
        public int NotifiedBeekeepersCount { get; set; }
        public string? WeatherWarning { get; set; }
    }
}