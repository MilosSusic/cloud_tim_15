namespace SmartApiary.Application.Interfaces.Messaging
{
    public interface IActivationQueueService
    {
        Task SendActivationAsync(string email, string activationToken, CancellationToken ct = default);
    }
}