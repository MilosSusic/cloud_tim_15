using Azure.Storage.Queues;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartApiary.Application;
using SmartApiary.Application.Features.SmartScales.Commands;
using SmartApiary.Domain.Common;
using SmartApiary.Functions.Extensions;
using SmartApiary.Infrastructure;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddHttpServices(builder.Configuration);

builder.Build().Run();

//builder.Services
//    .AddApplicationInsightsTelemetryWorkerService()
//    .ConfigureFunctionsApplicationInsights();

//builder.Services.AddInfrastructure(builder.Configuration);


//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
//    cfg.RegisterServicesFromAssembly(typeof(ProcessScaleTelemetryCommand).Assembly);
//});
//builder.Services.AddTransient<
//    MediatR.IRequestHandler<
//        SmartApiary.Application.Features.SmartScales.Commands.ActivateScaleCommand,
//        SmartApiary.Domain.Common.Result<string>>,
//    SmartApiary.Application.Features.SmartScales.Commands.ActivateScaleCommandHandler>();

//builder.Services.AddTransient<
//    MediatR.IRequestHandler<
//        SmartApiary.Application.Features.SmartScales.Commands.ProcessScaleTelemetryCommand,
//        SmartApiary.Domain.Common.Result>,
//    SmartApiary.Application.Features.SmartScales.Commands.ProcessScaleTelemetryHandler>();

//var app = builder.Build();

//await InitializeQueuesAsync(app.Services, builder.Configuration);

//app.Run();

//static async Task InitializeQueuesAsync(IServiceProvider services, IConfiguration configuration)
//{
//    var connectionString = configuration["AzureQueueOptions:ConnectionString"]
//        ?? configuration["AzureWebJobsStorage"];

//    if (string.IsNullOrWhiteSpace(connectionString))
//    {
//        return;
//    }

//    var queueNames = new[]
//    {
//        configuration["AzureQueueOptions:ResetPasswordQueue"],
//        configuration["AzureQueueOptions:ActivationQueue"],
//        configuration["AzureQueueOptions:SprayNotificationQueue"]
//    }
//    .Where(name => !string.IsNullOrWhiteSpace(name))
//    .Select(name => name!)
//    .Distinct(StringComparer.OrdinalIgnoreCase)
//    .ToArray();

//    if (queueNames.Length == 0)
//    {
//        return;
//    }

//    using var scope = services.CreateScope();
//    var logger = scope.ServiceProvider
//        .GetRequiredService<ILoggerFactory>()
//        .CreateLogger("QueueInitializer");

//    var queueServiceClient = new QueueServiceClient(connectionString);

//    foreach (var queueName in queueNames)
//    {
//        var queueClient = queueServiceClient.GetQueueClient(queueName);
//        await queueClient.CreateIfNotExistsAsync();
//        logger.LogInformation("Queue {QueueName} is ready.", queueName);
//    }
//}