using Microsoft.EntityFrameworkCore;
using SmartApiary.Domain.Models;

namespace SmartApiary.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.Property(x => x.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(x => x.Role)
                    .IsRequired();

                entity.Property(x => x.IsActive)
                    .IsRequired();

                entity.Property(x => x.ActivationToken)
                    .HasMaxLength(256);

                entity.Property(x => x.ActivationTokenExpiresAtUtc);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}