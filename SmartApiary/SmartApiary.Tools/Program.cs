using Azure.Storage.Queues;

const string connectionString = "UseDevelopmentStorage=true";

Console.WriteLine("--- SmartApiary Queue Initializer ---");
Console.WriteLine($"Target Storage: {connectionString}");
Console.WriteLine();

var queues = new[]
{
    "reset-password-queue",
    "activation-queue"
};

var queueServiceClient = new QueueServiceClient(connectionString);

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
