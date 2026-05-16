using Microsoft.EntityFrameworkCore;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Enums;
using SmartApiary.Infrastructure.Persistence;

namespace SmartApiary.Infrastructure.Repositories
{
    public class SprayNotificationRepository(ApplicationDbContext dbContext) : ISprayNotificationRepository
    {
        public async Task<SprayNotificationFieldRecipients?> GetFieldRecipientsInRadiusAsync(Guid fieldId, double radiusKm, CancellationToken ct = default)
        {
            var field = await dbContext.Fields
                .AsNoTracking()
                .Where(x => x.Id == fieldId)
                .Select(x => new { x.FieldName, x.Latitude, x.Longitude })
                .FirstOrDefaultAsync(ct);

            if (field is null)
            {
                return null;
            }

            var allApiaries = await dbContext.Apiaries
                .AsNoTracking()
                .Select(a => new { a.OwnerId, a.Latitude, a.Longitude })
                .ToListAsync(ct);

            var apiaryCountByOwner = allApiaries
                .Where(a => GetDistanceKm(a.Latitude, a.Longitude, field.Latitude, field.Longitude) <= radiusKm)
                .GroupBy(a => a.OwnerId)
                .ToDictionary(g => g.Key, g => g.Count());

            if (apiaryCountByOwner.Count == 0)
            {
                return new SprayNotificationFieldRecipients
                {
                    FieldName = field.FieldName
                };
            }

            var ownerIds = apiaryCountByOwner.Keys.ToList();

            var users = await dbContext.Users
                .AsNoTracking()
                .Where(u => ownerIds.Contains(u.Id) && u.Role == UserRole.Beekeeper && u.IsActive && !u.IsSuspended)
                .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email })
                .ToListAsync(ct);

            var recipients = users
                .Select(u => new SprayNotificationRecipient
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    NearbyApiaryCount = apiaryCountByOwner.TryGetValue(u.Id, out var count) ? count : 0
                })
                .ToList();

            return new SprayNotificationFieldRecipients
            {
                FieldName = field.FieldName,
                Recipients = recipients
            };
        }

        private static double GetDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double deg) => deg * (Math.PI / 180.0);
    }
}
