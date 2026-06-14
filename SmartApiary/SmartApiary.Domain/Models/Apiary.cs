using System;
using System.Collections.Generic;
using SmartApiary.Domain.Common;

namespace SmartApiary.Domain.Models
{
    public class Apiary
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? ThumbnailUrl { get; private set; }
        public Guid OwnerId { get; private set; }
        public List<Hive> Hives { get; private set; } = new();

        public static Result<Apiary> Create(
            Guid ownerId,
            string name,
            string? description,
            double latitude,
            double longitude,
            string? imageUrl,
            string? thumbnailUrl)
        {
            var ownerValidation = ValidateOwnerId(ownerId);
            if (ownerValidation.IsFailure)
                return Result<Apiary>.Failure(ownerValidation.Error!);

            var detailsValidation = ValidateDetails(name, description, latitude, longitude, imageUrl, thumbnailUrl);
            if (detailsValidation.IsFailure)
                return Result<Apiary>.Failure(detailsValidation.Error!);

            return Result<Apiary>.Success(new Apiary
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Name = name,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                ImageUrl = imageUrl,
                ThumbnailUrl = thumbnailUrl
            });
        }

        public Result UpdateDetails(
            string name,
            string? description,
            double latitude,
            double longitude,
            string? imageUrl,
            string? thumbnailUrl)
        {
            var detailsValidation = ValidateDetails(name, description, latitude, longitude, imageUrl, thumbnailUrl);
            if (detailsValidation.IsFailure)
                return detailsValidation;

            Name = name;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;

            if (!string.IsNullOrWhiteSpace(imageUrl))
                ImageUrl = imageUrl;

            if (!string.IsNullOrWhiteSpace(thumbnailUrl))
                ThumbnailUrl = thumbnailUrl;

            return Result.Success();
        }

        private static Result ValidateOwnerId(Guid ownerId)
        {
            if (ownerId == Guid.Empty)
                return Result.Failure("Owner id is required.");

            return Result.Success();
        }

        private static Result ValidateDetails(
            string name,
            string? description,
            double latitude,
            double longitude,
            string? imageUrl,
            string? thumbnailUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Apiary name is required.");
            if (name.Length > 100)
                return Result.Failure("Apiary name must not exceed 100 characters.");
            if (!string.IsNullOrWhiteSpace(description) && description.Length > 512)
                return Result.Failure("Description must not exceed 512 characters.");
            if (latitude < -90 || latitude > 90)
                return Result.Failure("Latitude must be between -90 and 90.");
            if (longitude < -180 || longitude > 180)
                return Result.Failure("Longitude must be between -180 and 180.");
            if (!string.IsNullOrWhiteSpace(imageUrl) && imageUrl.Length > 512)
                return Result.Failure("ImageUrl must not exceed 512 characters.");
            if (!string.IsNullOrWhiteSpace(thumbnailUrl) && thumbnailUrl.Length > 512)
                return Result.Failure("ThumbnailUrl must not exceed 512 characters.");

            return Result.Success();
        }
    }
}


