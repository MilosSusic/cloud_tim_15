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
using SmartApiary.Infrastructure.Extensions;
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
            services
                .AddPersistence(configuration)
                .AddAzureStorage(configuration)
                .AddCoreServices();

            return services;
        }
    }
}