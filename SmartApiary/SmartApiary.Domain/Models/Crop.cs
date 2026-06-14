using SmartApiary.Domain.Common;
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

        public static Result<Crop> Create(Guid fieldId, CropType cropName, string? description, DateTime floweringPeriodStart, DateTime floweringPeriodEnd)
        {
            var fieldValidation = ValidateFieldId(fieldId);
            if (fieldValidation.IsFailure)
                return Result<Crop>.Failure(fieldValidation.Error!);

            var detailsValidation = ValidateDetails(cropName, description, floweringPeriodStart, floweringPeriodEnd);
            if (detailsValidation.IsFailure)
                return Result<Crop>.Failure(detailsValidation.Error!);

            return Result<Crop>.Success(new Crop
            {
                Id = Guid.NewGuid(),
                FieldId = fieldId,
                CropName = cropName,
                Description = description,
                FloweringPeriodStart = floweringPeriodStart,
                FloweringPeriodEnd = floweringPeriodEnd,
                IsDeleted = false
            });
        }

        public Result Update(CropType cropName, string? description, DateTime floweringPeriodStart, DateTime floweringPeriodEnd)
        {
            var detailsValidation = ValidateDetails(cropName, description, floweringPeriodStart, floweringPeriodEnd);
            if (detailsValidation.IsFailure)
                return detailsValidation;

            CropName = cropName;
            Description = description;
            FloweringPeriodStart = floweringPeriodStart;
            FloweringPeriodEnd = floweringPeriodEnd;

            return Result.Success();
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        private static Result ValidateFieldId(Guid fieldId)
        {
            if (fieldId == Guid.Empty)
                return Result.Failure("Field id is required.");

            return Result.Success();
        }

        private static Result ValidateDetails(
            CropType cropName,
            string? description,
            DateTime floweringPeriodStart,
            DateTime floweringPeriodEnd)
        {
            if (!Enum.IsDefined(typeof(CropType), cropName))
                return Result.Failure("Invalid crop type.");
            if (!string.IsNullOrWhiteSpace(description) && description.Length > 512)
                return Result.Failure("Description must not exceed 512 characters.");
            if (floweringPeriodStart == default)
                return Result.Failure("Flowering period start is required.");
            if (floweringPeriodEnd == default)
                return Result.Failure("Flowering period end is required.");
            if (floweringPeriodStart > floweringPeriodEnd)
                return Result.Failure("Flowering period start must be before end.");

            return Result.Success();
        }
    }
}
