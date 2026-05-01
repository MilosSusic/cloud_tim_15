namespace SmartApiary.Application.Interfaces.Messaging
{
    public record SprayNotificationMessage(
        Guid SprayScheduleId,
        Guid FieldId,
        string NotificationType,
        DateTime StartDateTime,
        int DurationHours,
        string? PesticideType);
}
