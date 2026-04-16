namespace SmartApiary.Application.Interfaces.Messaging
{
    public interface IResetPasswordQueueService
    {
        Task SendResetPasswordAsync(string email, string resetPasswordToken, CancellationToken ct = default);
    }
}