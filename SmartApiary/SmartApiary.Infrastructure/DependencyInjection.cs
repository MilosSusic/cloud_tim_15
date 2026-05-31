using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SmartApiary.Application.Interfaces.Authentication;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Application.Interfaces.Storage;
using SmartApiary.Domain.Models;
using SmartApiary.Infrastructure.Authentication;
using SmartApiary.Infrastructure.Persistence;
using SmartApiary.Infrastructure.Persistence.AzureBlob.Services;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Services;
using SmartApiary.Infrastructure.Persistence.AzureSql.Repositories;
using SmartApiary.Infrastructure.Persistence.AzureTable.Common;
using SmartApiary.Infrastructure.Persistence.AzureTable.Entities;
using SmartApiary.Infrastructure.Persistence.AzureTable.KeyProviders;
using SmartApiary.Infrastructure.Persistence.AzureTable.Mappers;
using SmartApiary.Infrastructure.Persistence.AzureTable.Repositories;
using SmartApiary.Infrastructure.Repositories;
using SmartApiary.Infrastructure.Services;
using SmartApiary.Infrastructure.Services.Email;

namespace SmartApiary.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IApiaryRepository, ApiaryRepository>();
            services.AddScoped<IHiveRepository, HiveRepository>();
            services.AddScoped<IHiveLogEntryRepository, HiveLogEntryRepository>();
            services.AddScoped<IFieldRepository, FieldRepository>();
            services.AddScoped<ISprayScheduleRepository, SprayScheduleRepository>();
            services.AddScoped<ICropsRepository, CropsRepository>();
            services.AddScoped<ISprayNotificationRepository, SprayNotificationRepository>();
            services.AddScoped<ISmartScaleRepository, SmartScaleRepository>();

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IResetPasswordQueueService, ResetPasswordQueueService>();

            services.AddScoped<IActivationQueueService, ActivationQueueService>();
            services.AddScoped<ISprayNotificationQueueService, SprayNotificationQueueService>();

            services.AddScoped<IEmailService, SendGridEmailService>();
            services.AddScoped<ISprayNotificationEmailService, SprayNotificationEmailService>();
            services.AddScoped<ISprayNotificationOrchestrator, SprayNotificationOrchestrator>();

            services.AddHttpClient<IWeatherService, WeatherService>();

            services.AddScoped<ISprayLogRepository, SprayLogRepository>();

            services.AddHttpClient<IWeatherService, WeatherService>();

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

                var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<AzureBlobStorage>>();
                return new AzureBlobStorage(
                    apiaryConnectionString,
                    apiaryContainerName,
                    fieldConnectionString,
                    fieldContainerName,
                    logger);
            });

            services.AddScoped<IBlobStorageService>(sp => sp.GetRequiredService<AzureBlobStorage>());

            services.AddScoped<ITableKeyProvider<Telemetry>, TelemetryTableKeyProvider>();
            services.AddScoped<ITableMapper<Telemetry, TelemetryEntity>, TelemetryTableMapper>();
            services.AddScoped<ITelemetryRepository, TelemetryRepository>(sp =>
            {
                var connectionString = configuration["AzureTable:ConnectionString"]
                    ?? "UseDevelopmentStorage=true";

                var tableName = configuration["AzureTable:ScaleMeasurementsTable"] ?? throw new InvalidOperationException("AzureTable:Table name is not configured.");

                var keyProvider = sp.GetRequiredService<ITableKeyProvider<Telemetry>>();
                var mapper = sp.GetRequiredService<ITableMapper<Telemetry, TelemetryEntity>>();

                return new TelemetryRepository(connectionString, tableName, keyProvider, mapper);
            });


            return services;
        }
    }
}