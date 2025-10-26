using Health.Domain.Common;
using Health.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Health.Infrastructure.Data
{
    public class HealthDbContext : DbContext
    {
        public HealthDbContext(DbContextOptions<HealthDbContext> options)
            : base(options)
        {
        }

        // DbSets (User-only for first migration)
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(builder =>
            {
                // Keys
                builder.HasKey(u => u.UserId);

                
                builder.Property(u => u.FullName)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(u => u.Email)
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(u => u.PasswordHash)
                       .IsRequired();

                builder.Property(u => u.PhoneNumber)
                       .HasMaxLength(20);

                builder.Property(u => u.BloodType)
                       .HasMaxLength(10);

                builder.Property(u => u.Address)
                       .HasMaxLength(500);

                builder.Property(u => u.EmergencyContactName)
                       .HasMaxLength(100);

                builder.Property(u => u.EmergencyContactPhone)
                       .HasMaxLength(20);

                builder.Property(u => u.EmergencyContactRelationship)
                       .HasMaxLength(50);

                builder.Property(u => u.MedicalConditions)
                       .HasMaxLength(500);

                builder.Property(u => u.Allergies)
                       .HasMaxLength(500);

                builder.Property(u => u.CurrentMedications)
                       .HasMaxLength(500);

                // Enums stored as int
                builder.Property(u => u.Role).HasConversion<int>(); 
                builder.Property(u => u.Gender).HasConversion<int?>(); 

                // Indexes
                builder.HasIndex(u => u.Email).IsUnique(); 
                builder.HasIndex(u => u.IsDeleted);       
                builder.HasIndex(u => u.IsActive);         
            });

            // Optional: enable soft-delete filter now (you can add later if you prefer)
            // modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted); // global filter [web:32][web:72]
        }
    }
}
