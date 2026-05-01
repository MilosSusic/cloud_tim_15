namespace SmartApiary.Application.Interfaces.Messaging
{
    public interface ISprayNotificationQueueService
    {
        Task SendAsync(SprayNotificationMessage message, CancellationToken ct = default);
    }
}
