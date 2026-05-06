using Microsoft.EntityFrameworkCore;
using SmartApiary.Domain.Models;

namespace SmartApiary.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Apiary> Apiaries => Set<Apiary>();
        public DbSet<Hive> Hives => Set<Hive>();
        public DbSet<Field> Fields => Set<Field>();
        public DbSet<SpraySchedule> SpraySchedules => Set<SpraySchedule>();
        public DbSet<Crop> Crops => Set<Crop>();
        public DbSet<HiveLogEntry> HiveLogEntries => Set<HiveLogEntry>();
        public DbSet<SprayLog> SprayLogs => Set<SprayLog>();

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
                    .HasConversion<string>() // Enum kao string
                    .IsRequired();

                entity.Property(x => x.IsActive)
                    .IsRequired();

                entity.Property(x => x.IsSuspended)
                    .IsRequired();

                entity.Property(x => x.PhoneNumber)
                    .HasMaxLength(32);

                entity.Property(x => x.ActivationToken)
                    .HasMaxLength(256);

                entity.Property(x => x.ActivationTokenExpiresAtUtc);

                entity.Property(x => x.ResetPasswordToken)
                    .HasMaxLength(256);

                entity.Property(x => x.ResetPasswordTokenExpiresAtUtc);
            });

            modelBuilder.Entity<Apiary>(entity =>
            {
                entity.ToTable("Apiaries");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Description)
                    .HasMaxLength(512);

                entity.Property(x => x.Latitude)
                    .IsRequired();

                entity.Property(x => x.Longitude)
                    .IsRequired();

                entity.Property(x => x.ImageUrl)
                    .HasMaxLength(512);

                entity.Property(x => x.ThumbnailUrl)
                    .HasMaxLength(512);

                entity.Property(x => x.OwnerId)
                    .IsRequired();

                entity.HasMany(x => x.Hives)
                    .WithOne()
                    .HasForeignKey(x => x.ApiaryId);
            });

            modelBuilder.Entity<Hive>(entity =>
            {
                entity.ToTable("Hives");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.Label)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Color)
                    .HasMaxLength(50);

                entity.Property(x => x.QueenAge)
                    .IsRequired();

                entity.Property(x => x.Note)
                    .HasMaxLength(512);

                entity.Property(x => x.IsDeleted)
                    .IsRequired();

                entity.HasMany(x => x.LogEntries)
                    .WithOne()
                    .HasForeignKey(x => x.HiveId);
            });

            modelBuilder.Entity<Field>(entity =>
            {
                entity.ToTable("Fields");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FieldName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Description)
                    .HasMaxLength(512);

                entity.Property(x => x.Latitude)
                    .IsRequired();

                entity.Property(x => x.Longitude)
                    .IsRequired();

                entity.Property(x => x.ImageUrl)
                    .HasMaxLength(512);

                entity.Property(x => x.ThumbnailUrl)
                    .HasMaxLength(512);

                entity.Property(x => x.OwnerId)
                    .IsRequired();

                entity.HasMany(x => x.Crops)
                    .WithOne()
                    .HasForeignKey(x => x.FieldId);
                entity.HasMany(x => x.SpraySchedules)
                   .WithOne()
                   .HasForeignKey(x => x.FieldId);
            });
            modelBuilder.Entity<SpraySchedule>(entity =>
            {
                entity.ToTable("SpraySchedules");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FieldId)
                    .IsRequired();

                entity.Property(x => x.StartDateTime)
                    .IsRequired();

                entity.Property(x => x.DurationHours)
                    .IsRequired();

                entity.Property(x => x.PesticideType)
                    .HasMaxLength(100);
                entity.Property(x => x.Status)
                    .HasConversion<string>() 
                    .IsRequired();
            });

            modelBuilder.Entity<Crop>(entity =>
            {
                entity.ToTable("Crop");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.FieldId)
                    .IsRequired();
                entity.Property(x => x.CropName)
                    .IsRequired(); // store as int (enum)
                entity.Property(x => x.Description)
                    .HasMaxLength(512);
                entity.Property(x => x.FloweringPeriodStart)
                    .IsRequired();
                entity.Property(x => x.FloweringPeriodEnd)
                    .IsRequired();
                entity.Property(x => x.IsDeleted)
                    .IsRequired();
            });

            modelBuilder.Entity<SprayLog>(entity =>
            {
                entity.ToTable("SprayLogs");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.FieldName).IsRequired().HasMaxLength(100);
                entity.Property(x => x.CropName).HasMaxLength(100);
                entity.Property(x => x.PesticideType).HasMaxLength(100);
                entity.Property(x => x.WeatherDescription).HasMaxLength(256);
                entity.Property(x => x.WindSpeed);
                entity.Property(x => x.Temperature);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}