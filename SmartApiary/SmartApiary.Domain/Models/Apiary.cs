using System;
using System.Collections.Generic;

namespace SmartApiary.Domain.Models
{
    public class Apiary
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public Guid OwnerId { get; set; }
        public List<Hive> Hives { get; set; } = new();

        public static Apiary Create(
            Guid ownerId,
            string name,
            string? description,
            double latitude,
            double longitude,
            string? imageUrl,
            string? thumbnailUrl)
        {
            ValidateOwnerId(ownerId);
            ValidateDetails(name, description, latitude, longitude, imageUrl, thumbnailUrl);

            return new Apiary
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Name = name,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                ImageUrl = imageUrl,
                ThumbnailUrl = thumbnailUrl
            };
        }

        public void UpdateDetails(
            string name,
            string? description,
            double latitude,
            double longitude,
            string? imageUrl,
            string? thumbnailUrl)
        {
            ValidateDetails(name, description, latitude, longitude, imageUrl, thumbnailUrl);

            Name = name;
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
            string name,
            string? description,
            double latitude,
            double longitude,
            string? imageUrl,
            string? thumbnailUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Apiary name is required.", nameof(name));
            if (name.Length > 100)
                throw new ArgumentException("Apiary name must not exceed 100 characters.", nameof(name));
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


