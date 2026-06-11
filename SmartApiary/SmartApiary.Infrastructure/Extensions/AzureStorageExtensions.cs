using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Storage;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Persistence.AzureBlob.Services;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using SmartApiary.Infrastructure.Persistence.AzureTable.KeyProviders;
using SmartApiary.Infrastructure.Persistence.AzureTable.Mappers;
using SmartApiary.Infrastructure.Persistence.AzureTable.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Extensions
{
    public static class AzureStorageExtensions
    {
        public static IServiceCollection AddAzureStorage(this IServiceCollection services, IConfiguration configuration)
        {
            // Azure Blob Storage registracija
            services.AddScoped<AzureBlobStorage>(sp =>
            {
                var apiaryConnectionString = configuration["AzureBlob:ConnectionString"]
                    ?? throw new InvalidOperationException("AzureBlob:ConnectionString is not configured.");
                var apiaryContainerName = configuration["AzureBlob:Container"]
                    ?? throw new InvalidOperationException("AzureBlob:Container is not configured.");

                var fieldConnectionString = configuration["AzureBlob:FieldConnectionString"]
                    ?? throw new InvalidOperationException("AzureBlob:FieldConnectionString is not configured.");
                var fieldContainerName = configuration["AzureBlob:FieldContainer"]
                    ?? throw new InvalidOperationException("AzureBlob:FieldContainer is not configured.");

                var logger = sp.GetRequiredService<ILogger<AzureBlobStorage>>();

                return new AzureBlobStorage(
                    apiaryConnectionString,
                    apiaryContainerName,
                    fieldConnectionString,
                    fieldContainerName,
                    logger);
            });

            services.AddScoped<IBlobStorageService>(sp => sp.GetRequiredService<AzureBlobStorage>());

            // Azure Table Storage registracija
            services.AddScoped<ITableKeyProvider<Telemetry>, TelemetryTableKeyProvider>();
            services.AddScoped<ITableMapper<Telemetry, TelemetryEntity>, TelemetryTableMapper>();

            services.AddScoped<ITelemetryRepository, TelemetryRepository>(sp =>
            {
                var connectionString = configuration["AzureTable:ConnectionString"]
                    ?? "UseDevelopmentStorage=true";

                var tableName = configuration["AzureTable:ScaleMeasurementsTable"]
                    ?? throw new InvalidOperationException("AzureTable:Table name is not configured.");

                var keyProvider = sp.GetRequiredService<ITableKeyProvider<Telemetry>>();
                var mapper = sp.GetRequiredService<ITableMapper<Telemetry, TelemetryEntity>>();

                return new TelemetryRepository(connectionString, tableName, keyProvider, mapper);
            });

            // YieldRecord registracija
            services.AddScoped<ITableKeyProvider<YieldRecord>, YieldRecordKeyProvider>();
            services.AddScoped<ITableMapper<YieldRecord, YieldRecordEntity>, YieldRecordMapper>();

            services.AddScoped<IYieldRecordRepository, YieldRecordRepository>(sp =>
            {
                var connectionString = configuration["AzureTable:ConnectionString"]
                    ?? "UseDevelopmentStorage=true";

                var tableName = "YieldRecords";

                var keyProvider = sp.GetRequiredService<ITableKeyProvider<YieldRecord>>();
                var mapper = sp.GetRequiredService<ITableMapper<YieldRecord, YieldRecordEntity>>();

                return new YieldRecordRepository(connectionString, tableName, keyProvider, mapper);
            });

            // Azure Queue Storage registracija
            var queueConnectionString = configuration["AzureQueueOptions:ConnectionString"]
                ?? configuration["AzureWebJobsStorage"];

            services.AddSingleton(sp =>
            {
                return new QueueServiceClient(queueConnectionString, new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                });
            });

            return services;
        }
    }
}
