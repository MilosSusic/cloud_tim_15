using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartApiary.Infrastructure.Persistence.AzureQueue.Services
{
    internal class ResetPasswordQueueService : IResetPasswordQueueService
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<ResetPasswordQueueService> _logger;

        public ResetPasswordQueueService(
            IConfiguration configuration,
            ILogger<ResetPasswordQueueService> logger)
        {
            _logger = logger;

            var connectionString = configuration["AzureQueueOptions:ConnectionString"]
                ?? throw new InvalidOperationException("AzureQueueOptions:ConnectionString is not configured.");

            var queueName = configuration["AzureQueueOptions:ResetPasswordQueue"]
                ?? throw new InvalidOperationException("AzureQueueOptions:ResetPasswordQueue is not configured.");

            var options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            _queueClient = new QueueClient(connectionString, queueName, options);
        }

        public async Task SendResetPasswordAsync(string email, string resetPasswordToken, CancellationToken ct = default)
        {
            var message = new ResetPasswordMessage
            {
                Email = email,
                ResetPasswordToken = resetPasswordToken
            };

            var json = System.Text.Json.JsonSerializer.Serialize(message);

            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);
            await _queueClient.SendMessageAsync(json, cancellationToken: ct);

            _logger.LogInformation(
                "ResetPasswordMessage sent to queue {QueueName} for {Email}",
                _queueClient.Name,
                email);
        }
    }
}