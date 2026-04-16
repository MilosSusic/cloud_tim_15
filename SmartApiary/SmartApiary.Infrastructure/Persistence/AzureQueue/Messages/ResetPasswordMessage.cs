namespace SmartApiary.Infrastructure.Persistence.AzureQueue.Messages
{
    public class ResetPasswordMessage
    {
        public string Email { get; set; } = string.Empty;
        public string ResetPasswordToken { get; set; } = string.Empty;
    }
}