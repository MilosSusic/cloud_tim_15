using Microsoft.OpenApi.Models;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Infrastructure.Services.Email;
using SmartApiary.WebApi.Services;

namespace SmartApiary.WebApi.Extensions
{
    public static class WebServicesExtensions
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            // HttpContext i lokalni API servisi
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IScaleNotificationService, ScaleNotificationService>();
            services.AddScoped<IScaleAlertEmailService, ScaleAlertEmailService>();

            // CORS konfiguracija
            services.AddCors(options =>
            {
                options.AddPolicy("ReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // SignalR i standardni kontroleri
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSignalR();

            // Swagger sa Bearer token podrškom
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token."
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static async Task SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<SmartApiary.Infrastructure.Persistence.AzureSql.Seeders.ApplicationDbContextSeeder>();
            await seeder.SeedAsync();
        }
    }
}
