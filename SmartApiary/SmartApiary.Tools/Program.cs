using System;
using System.Threading.Tasks;
using SmartApiary.Infrastructure.Persistence.AzureBlob.Services;
using Azure.Storage.Queues;

namespace SmartApiary.Tools
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "upload-blob" && args.Length == 4)
            {
                var filePath = args[1];
                var connectionString = args[2];
                var containerName = args[3];
                var blobService = new AzureBlobStorage(connectionString, containerName);
                var url = await blobService.UploadFileAsync(filePath);
                Console.WriteLine($"Uploaded! Blob URL: {url}");
                return;
            }

            const string queueConnectionString = "UseDevelopmentStorage=true";

            Console.WriteLine("--- SmartApiary Queue Initializer ---");
            Console.WriteLine($"Target Storage: {queueConnectionString}");
            Console.WriteLine();

            var queues = new[]
            {
                "reset-password-queue",
                "activation-queue",
                "spray-notification-queue",
                "reset-password-queue-poison",
                "activation-queue-poison",
                "spray-notification-queue-poison"
            };

            var queueServiceClient = new QueueServiceClient(queueConnectionString);

            foreach (var queueName in queues)
            {
                try
                {
                    var queueClient = queueServiceClient.GetQueueClient(queueName);

                    Console.Write($"Deleting queue (if exists): {queueName}...");
                    await queueClient.DeleteIfExistsAsync();

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" [DELETED]");
                    Console.ResetColor();

                    Console.Write($"Creating fresh queue: {queueName}...");
                    await queueClient.CreateIfNotExistsAsync();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" [OK]");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($" [FAILED] - Error: {ex.Message}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            Console.WriteLine("--- Queue setup completed ---");

            Console.WriteLine("Usage:");
            Console.WriteLine("  upload-blob <filePath> <connectionString> <containerName>");
        }
    }
}
