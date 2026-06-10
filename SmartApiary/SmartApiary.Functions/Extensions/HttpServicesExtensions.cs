using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Functions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Functions.Extensions
{
    public static class HttpServicesExtensions
    {
        public static IServiceCollection AddHttpServices(this IServiceCollection services, IConfiguration configuration)
        {
            string webApiBaseUrl = configuration["WebApiBaseUrl"] ?? "https://localhost:7159/";

            services.AddHttpClient<IScaleNotificationService, HttpScaleNotificationService>(client =>
            {
                client.BaseAddress = new Uri(webApiBaseUrl);
            });
            services.AddScoped<ICurrentUserService, FakeCurrentUserService>();
            return services;
        }
    }
}
