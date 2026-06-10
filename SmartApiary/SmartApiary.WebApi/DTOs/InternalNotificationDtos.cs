namespace SmartApiary.WebApi.DTOs
{
    public class InternalTelemetryPayload
    {
        public Guid ApiaryId { get; set; }
        public string SmartScaleId { get; set; } = string.Empty;
        public double Weight { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public double BatteryLevel { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class InternalAnomalyPayload
    {
        public Guid ApiaryId { get; set; }
        public Guid ScaleId { get; set; }
        public string AnomalyType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public double CurrentValue { get; set; }
    }
}
