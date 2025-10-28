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

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<CaretakerAccess> CaretakerAccesses { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<AppointmentHistory> AppointmentHistories { get; set; } = null!;
        public DbSet<FileStorage> FileStorages { get; set; } = null!;
        public DbSet<Unit> Units { get; set; } = null!;
        public DbSet<Medication> Medications { get; set; } = null!;
        public DbSet<MedicationReminder> MedicationReminders { get; set; } = null!;
        public DbSet<ShareableLink> ShareableLinks { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        //public DbSet<HealthMetric> HealthMetrics { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(u => u.UserId);

                builder.Property(u => u.FullName).HasMaxLength(100).IsRequired();
                builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
                builder.Property(u => u.PasswordHash).IsRequired();
                builder.Property(u => u.PhoneNumber).HasMaxLength(20);
                builder.Property(u => u.BloodType).HasMaxLength(10);
                builder.Property(u => u.Address).HasMaxLength(500);

                builder.Property(u => u.EmergencyContactName).HasMaxLength(100);
                builder.Property(u => u.EmergencyContactPhone).HasMaxLength(20);
                builder.Property(u => u.EmergencyContactRelationship).HasMaxLength(50);
                builder.Property(u => u.MedicalConditions).HasMaxLength(500);
                builder.Property(u => u.Allergies).HasMaxLength(500);
                builder.Property(u => u.CurrentMedications).HasMaxLength(500);

                builder.Property(u => u.Role).HasConversion<int>();
                builder.Property(u => u.Gender).HasConversion<int?>();

                builder.HasIndex(u => u.Email).IsUnique();
                builder.HasIndex(u => u.IsDeleted);
                builder.HasIndex(u => u.IsActive);
            });

            // RefreshToken
            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasKey(x => x.TokenId);
                b.Property(x => x.Token).IsRequired();
                b.HasIndex(x => x.Token).IsUnique();

                b.HasOne(x => x.User)
                 .WithMany(u => u.RefreshTokens)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(x => x.IsDeleted);
            });

            // CaretakerAccess
            modelBuilder.Entity<CaretakerAccess>(b =>
            {
                b.HasKey(x => x.AccessId);

                b.HasOne(x => x.Patient)
                 .WithMany(u => u.Caretakers)
                 .HasForeignKey(x => x.PatientId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Caretaker)
                 .WithMany(u => u.PatientsUnderCare)
                 .HasForeignKey(x => x.CaretakerId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasCheckConstraint("CK_CaretakerAccess_Patient_NE_Caretaker", "[PatientId] <> [CaretakerId]");

                b.HasIndex(x => x.PatientId);
                b.HasIndex(x => x.CaretakerId);
                b.HasIndex(x => new { x.PatientId, x.CaretakerId }).IsUnique(false);
                b.HasIndex(x => x.IsActive);
                b.HasIndex(x => x.IsDeleted);

                b.Property(x => x.Relationship).HasConversion<int>();
                b.Property(x => x.AccessLevel).HasConversion<int>();
                b.Property(x => x.Notes).HasMaxLength(500);
            });

            // Appointment
            modelBuilder.Entity<Appointment>(b =>
            {
                b.HasKey(x => x.AppointmentId);

                b.Property(x => x.Status).HasConversion<int>();
                b.Property(x => x.PatientNotes).HasMaxLength(500);
                b.Property(x => x.DoctorNotes);
                b.Property(x => x.RejectionReason).HasMaxLength(500);
                b.Property(x => x.Hospital).HasMaxLength(200);
                b.Property(x => x.Location).HasMaxLength(300);

                b.HasOne(x => x.Patient)
                 .WithMany(u => u.AppointmentsAsPatient)
                 .HasForeignKey(x => x.PatientId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Doctor)
                 .WithMany(u => u.AppointmentsAsDoctor)
                 .HasForeignKey(x => x.DoctorId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => x.PatientId);
                b.HasIndex(x => x.DoctorId);
                b.HasIndex(x => x.AppointmentDate);
                b.HasIndex(x => x.Status);
                b.HasIndex(x => x.IsDeleted);
            });

            // AppointmentHistory
            modelBuilder.Entity<AppointmentHistory>(b =>
            {
                b.HasKey(x => x.HistoryId);

                b.Property(x => x.Action).HasMaxLength(50).IsRequired();
                b.Property(x => x.Reason).HasMaxLength(500);

                b.HasOne(x => x.Appointment)
                 .WithMany(a => a.History)
                 .HasForeignKey(x => x.AppointmentId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.ChangedByUser)
                 .WithMany()
                 .HasForeignKey(x => x.ChangedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => x.AppointmentId);
                b.HasIndex(x => x.ChangedByUserId);
                b.HasIndex(x => x.IsDeleted);
            });

            // FileStorage
            modelBuilder.Entity<FileStorage>(b =>
            {
                b.HasKey(x => x.FileStorageId);

                b.Property(x => x.FileName).HasMaxLength(255).IsRequired();
                b.Property(x => x.FileExtension).HasMaxLength(10).IsRequired();
                b.Property(x => x.FileData).IsRequired();
                b.Property(x => x.ContentType).HasMaxLength(100);

                b.HasOne(x => x.UploadedByUser)
                 .WithMany(u => u.UploadedFiles)
                 .HasForeignKey(x => x.UploadedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => x.UploadedByUserId);
                b.HasIndex(x => x.IsDeleted);
            });

            // MedicalRecord
            modelBuilder.Entity<MedicalRecord>(b =>
            {
                b.HasKey(x => x.RecordId);

                b.Property(x => x.Title).HasMaxLength(200).IsRequired();
                b.Property(x => x.Hospital).HasMaxLength(200);

                b.HasOne(x => x.User)
                 .WithMany(u => u.MedicalRecords)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.File)
                 .WithMany()
                 .HasForeignKey(x => x.FileStorageId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.RecordDate);
                b.HasIndex(x => x.IsDeleted);
            });

            // Unit (UCUM-aligned)
            modelBuilder.Entity<Unit>(b =>
            {
                b.HasKey(x => x.UnitId);
                b.Property(x => x.UnitName).HasMaxLength(20).IsRequired(); // e.g., mg, mL, IU
                b.Property(x => x.Description).HasMaxLength(100);

                b.HasIndex(x => x.UnitName).IsUnique(); // avoid duplicates

                
                b.HasData(
                    new Unit { UnitId = 1, UnitName = "mg", Description = "milligram", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Unit { UnitId = 2, UnitName = "g", Description = "gram", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Unit { UnitId = 3, UnitName = "mcg", Description = "microgram", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Unit { UnitId = 4, UnitName = "ml", Description = "milliliter", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Unit { UnitId = 5, UnitName = "L", Description = "liter", CreatedOn = DateTime.UtcNow, IsDeleted = false },
                    new Unit { UnitId = 6, UnitName = "IU", Description = "international unit", CreatedOn = DateTime.UtcNow, IsDeleted = false }
                );
            });

            // Medication
            modelBuilder.Entity<Medication>(b =>
            {
                b.HasKey(x => x.MedicationId);

                b.Property(x => x.Name).HasMaxLength(150).IsRequired();
                b.Property(x => x.Instructions).HasMaxLength(500);
                b.Property(x => x.Frequency).HasMaxLength(100);

                // Explicit decimal precision
                b.Property(x => x.DosageValue).HasPrecision(10, 2);
                b.Property(x => x.DoseRangeLow).HasPrecision(10, 2);
                b.Property(x => x.DoseRangeHigh).HasPrecision(10, 2);

                b.HasOne(x => x.User)
                 .WithMany(u => u.Medications)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Primary dosage unit (inverse navigations present on Unit)
                b.HasOne(x => x.Unit)
                 .WithMany(u => u.Medications)
                 .HasForeignKey(x => x.UnitId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Dose range unit (optional)
                b.HasOne(x => x.DoseRangeUnit)
                 .WithMany(u => u.DoseRangeMedications)
                 .HasForeignKey(x => x.DoseRangeUnitId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.UnitId);
                b.HasIndex(x => x.StartDate);
                b.HasIndex(x => x.IsDeleted);
            });

            // MedicationReminder
            
            modelBuilder.Entity<MedicationReminder>(b =>
            {
                b.HasKey(x => x.ReminderId);

                b.Property(x => x.Channel).HasMaxLength(20);
                b.Property(x => x.Notes).HasMaxLength(300);

                b.HasOne(x => x.Medication)
                 .WithMany(m => m.Reminders)
                 .HasForeignKey(x => x.MedicationId)
                 .OnDelete(DeleteBehavior.Cascade); // delete reminders with parent med [web:136]

                // Helpful indexes for scheduler queries
                b.HasIndex(x => x.MedicationId);
                b.HasIndex(x => x.RemindAt);  
                b.HasIndex(x => x.IsSent);
                b.HasIndex(x => x.IsDeleted);
            });

            modelBuilder.Entity<ShareableLink>(b =>
            {
                b.HasKey(x => x.ShareableLinkId);

                b.Property(x => x.Token).HasMaxLength(128).IsRequired();
                b.HasIndex(x => x.Token).IsUnique();                 // enforce uniqueness [web:331][web:327]

                
                b.Property(x => x.ExpiresAt).IsRequired();

                b.HasOne(x => x.User)
                 .WithMany(u => u.ShareableLinks)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);                  // cleanup with user [web:136]

                // Helpful indexes
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.IsActive);
                b.HasIndex(x => x.ExpiresAt);                        // TTL queries
                b.HasIndex(x => x.IsDeleted);
            });

            modelBuilder.Entity<Notification>(b =>
            {
                b.HasKey(x => x.NotificationId);

                b.Property(x => x.Title).HasMaxLength(120).IsRequired();
                b.Property(x => x.Message).HasMaxLength(1000).IsRequired();
                b.Property(x => x.Category).HasMaxLength(40);
                b.Property(x => x.ActionUrl).HasMaxLength(300);

                b.HasOne(x => x.User)
                 .WithMany(u => u.Notifications)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade); // remove notifications if user deleted [web:136]

                // Useful indexes for inbox queries
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => new { x.UserId, x.IsRead });
                b.HasIndex(x => x.Priority);
                b.HasIndex(x => x.IsDeleted);
            });

            //modelBuilder.Entity<HealthMetric>(b =>
            //{
            //    b.HasKey(x => x.HealthMetricId);

            //    b.Property(x => x.MetricCode).HasMaxLength(50).IsRequired();
            //    b.Property(x => x.Unit).HasMaxLength(20).IsRequired();
            //    b.Property(x => x.Value).HasPrecision(10, 2);     // decimal(10,2) [web:269][web:278]
            //    b.Property(x => x.Notes).HasMaxLength(300);

            //    b.HasOne(x => x.User)
            //     .WithMany(u => u.HealthMetrics)
            //     .HasForeignKey(x => x.UserId)
            //     .OnDelete(DeleteBehavior.Cascade);               // tie to patient [web:136]

            //    // Helpful indexes
            //    b.HasIndex(x => x.UserId);
            //    b.HasIndex(x => x.MetricCode);
            //    b.HasIndex(x => x.MeasuredAt);
            //    b.HasIndex(x => x.IsDeleted);
            //});


        }
    }
}
