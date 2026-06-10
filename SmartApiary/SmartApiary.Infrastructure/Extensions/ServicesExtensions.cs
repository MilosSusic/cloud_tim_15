using Microsoft.Extensions.DependencyInjection;
using SmartApiary.Application.Interfaces.Authentication;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Infrastructure.Authentication;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Services;
using SmartApiary.Infrastructure.Services;
using SmartApiary.Infrastructure.Services.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IResetPasswordQueueService, ResetPasswordQueueService>();
            services.AddScoped<IActivationQueueService, ActivationQueueService>();
            services.AddScoped<ISprayNotificationQueueService, SprayNotificationQueueService>();

            services.AddScoped<IEmailService, SendGridEmailService>();
            services.AddScoped<ISprayNotificationEmailService, SprayNotificationEmailService>();
            services.AddScoped<ISprayNotificationOrchestrator, SprayNotificationOrchestrator>();
            services.AddScoped<IScaleAlertEmailService, ScaleAlertEmailService>();

            services.AddHttpClient<IWeatherService, WeatherService>();

            return services;
        }
    }
}
