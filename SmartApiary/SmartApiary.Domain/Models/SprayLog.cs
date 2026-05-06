namespace SmartApiary.Domain.Models
{
    public class SprayLog
    {
        public Guid Id { get; set; }
        public Guid SprayScheduleId { get; set; }
        public Guid FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string? CropName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? PesticideType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double? WindSpeed { get; set; }
        public double? Temperature { get; set; }
        public string? WeatherDescription { get; set; }
    }
}