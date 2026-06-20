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