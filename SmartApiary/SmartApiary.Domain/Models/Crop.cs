using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (floweringPeriodStart > floweringPeriodEnd)
            {
                var temp = floweringPeriodStart;
                floweringPeriodStart = floweringPeriodEnd;
                floweringPeriodEnd = temp;
            }

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
            if (floweringPeriodStart > floweringPeriodEnd)
            {
                var temp = floweringPeriodStart;
                floweringPeriodStart = floweringPeriodEnd;
                floweringPeriodEnd = temp;
            }
            CropName = cropName;
            Description = description;
            FloweringPeriodStart = floweringPeriodStart;
            FloweringPeriodEnd = floweringPeriodEnd;
        }
        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
    }
}
