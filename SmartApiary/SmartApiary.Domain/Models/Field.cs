using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.Models
{
    public class Field
    {
        public Guid Id { get; private set; }
        public string FieldName { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public Guid OwnerId { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public List<Crop> Crops { get; private set; } = new();
        public List<SpraySchedule> SpraySchedules { get; private set; } = new();
        public static Field Create(Guid ownerId, string fieldName, string? description, double latitude, double longitude, string? imageUrl, string? thumbnailUrl)
        {
            return new Field
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                FieldName = fieldName,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                ImageUrl = imageUrl,
                ThumbnailUrl = thumbnailUrl
            };
        }
        public void Update(string fieldName, string? description, double latitude, double longitude, string? imageUrl, string? thumbnailUrl)
        {
            FieldName = fieldName;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;

            if (!string.IsNullOrWhiteSpace(imageUrl))
                ImageUrl = imageUrl;

            if (!string.IsNullOrWhiteSpace(thumbnailUrl))
                ThumbnailUrl = thumbnailUrl;
        }

    }
}
