using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Infrastructure.Persistence;
using SmartApiary.Infrastructure.Persistence.AzureSql.Repositories;
using SmartApiary.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Extensions
{
    public static class PersistenceExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<SmartApiary.Infrastructure.Persistence.AzureSql.Seeders.ApplicationDbContextSeeder>();
            services.AddScoped<IApiaryRepository, ApiaryRepository>();
            services.AddScoped<IHiveRepository, HiveRepository>();
            services.AddScoped<IHiveLogEntryRepository, HiveLogEntryRepository>();
            services.AddScoped<IFieldRepository, FieldRepository>();
            services.AddScoped<ISprayScheduleRepository, SprayScheduleRepository>();
            services.AddScoped<ICropsRepository, CropsRepository>();
            services.AddScoped<ISprayNotificationRepository, SprayNotificationRepository>();
            services.AddScoped<ISmartScaleRepository, SmartScaleRepository>();
            services.AddScoped<ISprayLogRepository, SprayLogRepository>();

            return services;
        }
    }
}
