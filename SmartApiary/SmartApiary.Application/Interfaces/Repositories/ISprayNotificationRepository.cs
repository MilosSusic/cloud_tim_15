namespace SmartApiary.Application.Interfaces.Repositories
{
    public interface ISprayNotificationRepository
    {
        Task<SprayNotificationFieldRecipients?> GetFieldRecipientsInRadiusAsync(Guid fieldId, double radiusKm, CancellationToken ct = default);
    }

    public class SprayNotificationFieldRecipients
    {
        public string FieldName { get; set; } = string.Empty;
        public List<SprayNotificationRecipient> Recipients { get; set; } = new();
    }

    public class SprayNotificationRecipient
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int NearbyApiaryCount { get; set; }
    }
}
