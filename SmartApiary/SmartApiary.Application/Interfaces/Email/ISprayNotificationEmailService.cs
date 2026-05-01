namespace SmartApiary.Application.Interfaces.Email
{
    public interface ISprayNotificationEmailService
    {
        Task SendScheduledAsync(string to, string firstName, string lastName, string fieldName, DateTime startDateTime, int durationHours, string? pesticideType, int nearbyApiaryCount);
        Task SendRescheduledAsync(string to, string firstName, string lastName, string fieldName, DateTime startDateTime, int durationHours, string? pesticideType, int nearbyApiaryCount);
        Task SendCancelledAsync(string to, string firstName, string lastName, string fieldName, DateTime startDateTime, int durationHours, string? pesticideType, int nearbyApiaryCount);
    }
}
