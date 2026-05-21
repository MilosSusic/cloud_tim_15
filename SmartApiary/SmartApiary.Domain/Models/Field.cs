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
            ValidateOwnerId(ownerId);
            ValidateDetails(fieldName, description, latitude, longitude, imageUrl, thumbnailUrl);

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
            ValidateDetails(fieldName, description, latitude, longitude, imageUrl, thumbnailUrl);

            FieldName = fieldName;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;

            if (!string.IsNullOrWhiteSpace(imageUrl))
                ImageUrl = imageUrl;

            if (!string.IsNullOrWhiteSpace(thumbnailUrl))
                ThumbnailUrl = thumbnailUrl;
        }

        private static void ValidateOwnerId(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
                throw new ArgumentException("Owner id is required.", nameof(ownerId));
        }

        private static void ValidateDetails(
            string fieldName,
            string? description,
            double latitude,
            double longitude,
            string? imageUrl,
            string? thumbnailUrl)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException("Field name is required.", nameof(fieldName));
            if (fieldName.Length > 100)
                throw new ArgumentException("Field name must not exceed 100 characters.", nameof(fieldName));
            if (!string.IsNullOrWhiteSpace(description) && description.Length > 512)
                throw new ArgumentException("Description must not exceed 512 characters.", nameof(description));
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));
            if (!string.IsNullOrWhiteSpace(imageUrl) && imageUrl.Length > 512)
                throw new ArgumentException("ImageUrl must not exceed 512 characters.", nameof(imageUrl));
            if (!string.IsNullOrWhiteSpace(thumbnailUrl) && thumbnailUrl.Length > 512)
                throw new ArgumentException("ThumbnailUrl must not exceed 512 characters.", nameof(thumbnailUrl));
        }
    }
}
