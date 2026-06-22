namespace SmartApiary.WebApi.DTOs
{
    public class InternalYieldPayload
    {
        public Guid ApiaryId { get; set; }
        public string ScaleId { get; set; }
        public double DeltaKg { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
