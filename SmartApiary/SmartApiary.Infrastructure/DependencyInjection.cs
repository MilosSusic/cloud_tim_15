using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using SmartApiary.Application.Interfaces.Authentication;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;

using SmartApiary.Infrastructure.Authentication;
using SmartApiary.Infrastructure.Persistence;
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

            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<IResetPasswordQueueService, ResetPasswordQueueService>();

            services.AddScoped<IActivationQueueService, ActivationQueueService>();

            services.AddScoped<IEmailService, SendGridEmailService>();

            return services;
        }
    }
}