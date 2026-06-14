using SmartApiary.Domain.Common;

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

        public static Result<SprayLog> Create(
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
            var validation = Validate(
                sprayScheduleId,
                fieldId,
                fieldName,
                cropName,
                startDateTime,
                endDateTime,
                pesticideType,
                weatherDescription);

            if (validation.IsFailure)
                return Result<SprayLog>.Failure(validation.Error!);

            return Result<SprayLog>.Success(new SprayLog
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
            });
        }

        private static Result Validate(
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
                return Result.Failure("Spray schedule id is required.");
            if (fieldId == Guid.Empty)
                return Result.Failure("Field id is required.");
            if (string.IsNullOrWhiteSpace(fieldName))
                return Result.Failure("Field name is required.");
            if (fieldName.Length > 100)
                return Result.Failure("Field name must not exceed 100 characters.");
            if (!string.IsNullOrWhiteSpace(cropName) && cropName.Length > 100)
                return Result.Failure("Crop name must not exceed 100 characters.");
            if (startDateTime == default)
                return Result.Failure("Start date is required.");
            if (endDateTime == default)
                return Result.Failure("End date is required.");
            if (endDateTime < startDateTime)
                return Result.Failure("End date must be after start date.");
            if (!string.IsNullOrWhiteSpace(pesticideType) && pesticideType.Length > 100)
                return Result.Failure("Pesticide type must not exceed 100 characters.");
            if (!string.IsNullOrWhiteSpace(weatherDescription) && weatherDescription.Length > 256)
                return Result.Failure("Weather description must not exceed 256 characters.");

            return Result.Success();
        }
    }
}