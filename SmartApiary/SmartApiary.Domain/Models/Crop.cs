using SmartApiary.Domain.Enums;

namespace SmartApiary.Domain.Models
{
    public class Crop
    {
        public Guid Id { get; private set; }
        public Guid FieldId { get; private set; }
        public CropType CropName { get; private set; }
        public DateTime FloweringPeriodStart { get; private set; }
        public DateTime FloweringPeriodEnd { get; private set; }
        public string? Description { get; private set; }
        public bool IsDeleted { get; private set; }

        public static Crop Create(Guid fieldId, CropType cropName, string? description, DateTime floweringPeriodStart, DateTime floweringPeriodEnd)
        {
            ValidateFieldId(fieldId);
            ValidateDetails(cropName, description, floweringPeriodStart, floweringPeriodEnd);

            return new Crop
            {
                Id = Guid.NewGuid(),
                FieldId = fieldId,
                CropName = cropName,
                Description = description,
                FloweringPeriodStart = floweringPeriodStart,
                FloweringPeriodEnd = floweringPeriodEnd,
                IsDeleted = false
            };
        }

        public void Update(CropType cropName, string? description, DateTime floweringPeriodStart, DateTime floweringPeriodEnd)
        {
            ValidateDetails(cropName, description, floweringPeriodStart, floweringPeriodEnd);

            CropName = cropName;
            Description = description;
            FloweringPeriodStart = floweringPeriodStart;
            FloweringPeriodEnd = floweringPeriodEnd;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        private static void ValidateFieldId(Guid fieldId)
        {
            if (fieldId == Guid.Empty)
                throw new ArgumentException("Field id is required.", nameof(fieldId));
        }

        private static void ValidateDetails(
            CropType cropName,
            string? description,
            DateTime floweringPeriodStart,
            DateTime floweringPeriodEnd)
        {
            if (!Enum.IsDefined(typeof(CropType), cropName))
                throw new ArgumentException("Invalid crop type.", nameof(cropName));
            if (!string.IsNullOrWhiteSpace(description) && description.Length > 512)
                throw new ArgumentException("Description must not exceed 512 characters.", nameof(description));
            if (floweringPeriodStart == default)
                throw new ArgumentException("Flowering period start is required.", nameof(floweringPeriodStart));
            if (floweringPeriodEnd == default)
                throw new ArgumentException("Flowering period end is required.", nameof(floweringPeriodEnd));
            if (floweringPeriodStart > floweringPeriodEnd)
                throw new ArgumentException("Flowering period start must be before end.", nameof(floweringPeriodStart));
        }
    }
}
