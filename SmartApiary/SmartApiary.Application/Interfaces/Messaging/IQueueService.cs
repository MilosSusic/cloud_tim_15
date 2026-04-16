namespace SmartApiary.Application.Interfaces.Messaging
{
    public interface IQueueService
    {
        Task SendAsync<T>(string queueName, T message, CancellationToken ct = default);
    }
}