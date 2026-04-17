namespace SmartApiary.Infrastructure.Persistence.AzureQueue.Messages
{
    public class ActivationMessage
    {
        public string Email { get; set; } = string.Empty;
        public string ActivationToken { get; set; } = string.Empty;
    }
}