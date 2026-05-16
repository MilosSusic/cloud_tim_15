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

using SmartApiary.Infrastructure.Authentication;
using SmartApiary.Infrastructure.Persistence;
using SmartApiary.Infrastructure.Persistence.AzureBlob.Services;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Services;
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

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IResetPasswordQueueService, ResetPasswordQueueService>();

            services.AddScoped<IActivationQueueService, ActivationQueueService>();
            services.AddScoped<ISprayNotificationQueueService, SprayNotificationQueueService>();

            services.AddScoped<IEmailService, SendGridEmailService>();
            services.AddScoped<ISprayNotificationEmailService, SprayNotificationEmailService>();

            services.AddHttpClient<IWeatherService, WeatherService>();

            services.AddScoped<ISprayLogRepository, SprayLogRepository>();

            services.AddHttpClient<IWeatherService, WeatherService>();

            services.AddScoped<IBlobStorageService>(_ =>
            {
                var connectionString = configuration["AzureBlob:ConnectionString"]
                    ?? throw new InvalidOperationException("AzureBlob:ConnectionString is not configured.");
                var containerName = configuration["AzureBlob:Container"]
                    ?? throw new InvalidOperationException("AzureBlob:Container is not configured.");
                return new BlobUploadService(connectionString, containerName);
            });

            services.AddScoped<IFieldBlobStorageService>(_ =>
            {
                var connectionString = configuration["AzureBlob:FieldConnectionString"]
                    ?? throw new InvalidOperationException("AzureBlob:FieldConnectionString is not configured.");
                var containerName = configuration["AzureBlob:FieldContainer"]
                    ?? throw new InvalidOperationException("AzureBlob:FieldContainer is not configured.");
                return new FieldBlobUploadService(connectionString, containerName);
            });

            return services;
        }
    }
}