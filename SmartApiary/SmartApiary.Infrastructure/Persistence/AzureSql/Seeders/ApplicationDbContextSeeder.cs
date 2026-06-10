using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Persistence.AzureSql.Seeders
{
    public class ApplicationDbContextSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<ApplicationDbContextSeeder> _logger;

        public ApplicationDbContextSeeder(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger<ApplicationDbContextSeeder> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                var adminEmail = "admin@test.com";
                var existingAdmin = await _userRepository.GetByEmailAsync(adminEmail);

                if (existingAdmin is null)
                {
                    var admin = User.Create(
                        "Admin",
                        "Test",
                        adminEmail,
                        UserRole.Admin,
                        Guid.NewGuid().ToString(),
                        DateTime.UtcNow.AddDays(1));

                    // Aktivacija admina i postavljanje hesirane lozinke
                    admin.Activate(_passwordHasher.Hash("Admin123!"));

                    // Cuvanje kroz Repository obrazac
                    await _userRepository.AddAsync(admin);
                    await _userRepository.SaveChangesAsync();

                    _logger.LogInformation("Database successfully seeded: Default Admin user created.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}
