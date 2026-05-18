namespace SmartApiary.Domain.Models
{
    public class SprayLog
    {
        public Guid Id { get; private set; }
        public Guid SprayScheduleId { get; private set; }
        public Guid FieldId { get; private set; }
        public string FieldName { get; private set; } = string.Empty;
        public string? CropName { get; private set; }
        public DateTime StartDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }
        public string? PesticideType { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public double? WindSpeed { get; private set; }
        public double? Temperature { get; private set; }
        public string? WeatherDescription { get; private set; }

        public static SprayLog Create(
            Guid sprayScheduleId,
            Guid fieldId,
            string fieldName,
            string? cropName,
            DateTime startDateTime,
            DateTime endDateTime,
            string? pesticideType,
            double? windSpeed,
            double? temperature,
            string? weatherDescription)
        {
            Validate(
                sprayScheduleId,
                fieldId,
                fieldName,
                cropName,
                startDateTime,
                endDateTime,
                pesticideType,
                weatherDescription);

            return new SprayLog
            {
                Id = Guid.NewGuid(),
                SprayScheduleId = sprayScheduleId,
                FieldId = fieldId,
                FieldName = fieldName,
                CropName = cropName,
                StartDateTime = startDateTime,
                EndDateTime = endDateTime,
                PesticideType = pesticideType,
                WindSpeed = windSpeed,
                Temperature = temperature,
                WeatherDescription = weatherDescription
            };
        }

        private static void Validate(
            Guid sprayScheduleId,
            Guid fieldId,
            string fieldName,
            string? cropName,
            DateTime startDateTime,
            DateTime endDateTime,
            string? pesticideType,
            string? weatherDescription)
        {
            if (sprayScheduleId == Guid.Empty)
                throw new ArgumentException("Spray schedule id is required.", nameof(sprayScheduleId));
            if (fieldId == Guid.Empty)
                throw new ArgumentException("Field id is required.", nameof(fieldId));
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException("Field name is required.", nameof(fieldName));
            if (fieldName.Length > 100)
                throw new ArgumentException("Field name must not exceed 100 characters.", nameof(fieldName));
            if (!string.IsNullOrWhiteSpace(cropName) && cropName.Length > 100)
                throw new ArgumentException("Crop name must not exceed 100 characters.", nameof(cropName));
            if (startDateTime == default)
                throw new ArgumentException("Start date is required.", nameof(startDateTime));
            if (endDateTime == default)
                throw new ArgumentException("End date is required.", nameof(endDateTime));
            if (endDateTime < startDateTime)
                throw new ArgumentException("End date must be after start date.", nameof(endDateTime));
            if (!string.IsNullOrWhiteSpace(pesticideType) && pesticideType.Length > 100)
                throw new ArgumentException("Pesticide type must not exceed 100 characters.", nameof(pesticideType));
            if (!string.IsNullOrWhiteSpace(weatherDescription) && weatherDescription.Length > 256)
                throw new ArgumentException("Weather description must not exceed 256 characters.", nameof(weatherDescription));
        }
    }
}