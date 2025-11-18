using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Health.Domain.Common;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Health.Infrastructure.Data
{
    public class HealthDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HealthDbContext(
            DbContextOptions<HealthDbContext> options,
            IHttpContextAccessor httpContextAccessor
        ) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
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
        public DbSet<HealthMetric> HealthMetrics { get; set; } = null!;
        public DbSet<OtpVerification> OtpVerifications { get; set; } = null!;
        public DbSet<BloodType> BloodTypes { get; set; } = null!;
        public DbSet<Consultation> Consultations { get; set; } = null!;

        public DbSet<Prescription> Prescriptions { get; set; } = null!;
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; } = null!;
        public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public DbSet<RecordType> RecordTypes { get; set; } = null!;

        //  Automatically
        //  set CreatedBy, ModifiedBy, DeletedBy
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.CreatedBy = userId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedOn = DateTime.UtcNow;
                        entry.Entity.ModifiedBy = userId;
                        break;

                    case EntityState.Deleted:
                        // Soft delete pattern
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedOn = DateTime.UtcNow;
                        entry.Entity.DeletedBy = userId;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        //  Extract current user ID from JWT
        private int? GetCurrentUserId()
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                    return userId;
            }
            catch
            {
                // ignored (for background services or unauthenticated operations)
            }
            return null;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(u => u.UserId);

                builder.Property(u => u.FullName).HasMaxLength(100).IsRequired();
                builder.Property(u => u.Email).HasMaxLength(100).IsRequired();
                builder.Property(u => u.PasswordHash).IsRequired(false); // Made nullable for OTP-only caretakers
                builder.Property(u => u.PhoneNumber).HasMaxLength(20);
                builder.Property(u => u.Status)
                                .HasConversion<int>()
                                .HasDefaultValue(AccountStatus.Pending)
                                .IsRequired();

                //builder.Property(u => u.BloodType).HasMaxLength(10);
                builder.HasOne(u => u.BloodType)
                                    .WithMany(bt => bt.Users)
                                    .HasForeignKey(u => u.BloodTypeId)
                                    .OnDelete(DeleteBehavior.SetNull);
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

                // Prevent duplicate bookings
                b.HasIndex(x => new { x.DoctorId, x.AppointmentDate, x.AppointmentTime })
                 .IsUnique()
                 .HasDatabaseName("UX_Doctor_AppointmentSlot");

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
                b.ToTable("FileStorage");
                b.HasKey(x => x.FileStorageId);

                b.Property(x => x.FileName).IsRequired().HasMaxLength(255);
                b.Property(x => x.FileExtension).IsRequired().HasMaxLength(50);
                b.Property(x => x.ContentType).HasMaxLength(200);
                b.Property(x => x.FileData).IsRequired();
                b.Property(x => x.FileSize);

                // Link to Consultation
                b.HasOne(x => x.Consultation)
                 .WithMany(c => c.Files)
                 .HasForeignKey(x => x.ConsultationId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Link to User (uploaded by)
                b.HasOne(x => x.UploadedByUser)
                 .WithMany(u => u.UploadedFiles)
                 .HasForeignKey(x => x.UploadedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => x.ConsultationId);
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

            // Unit 
            modelBuilder.Entity<Unit>(b =>
            {
                b.HasKey(x => x.UnitId);
                b.Property(x => x.UnitName).HasMaxLength(20).IsRequired();
                b.Property(x => x.Description).HasMaxLength(100);

                b.Property(x => x.CreatedOn)
                 .HasDefaultValueSql("GETUTCDATE()");

                b.Property(x => x.CreatedBy)
                 .HasDefaultValue(1);

                b.HasIndex(x => x.UnitName).IsUnique();

                // Deterministic HasData call
                b.HasData(UnitData.GetSeedUnits());
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
                 .OnDelete(DeleteBehavior.Cascade); // delete reminders with parent med

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
                b.HasIndex(x => x.Token).IsUnique();

                b.Property(x => x.ExpiresAt).IsRequired();

                b.HasOne(x => x.User)
                 .WithMany(u => u.ShareableLinks)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Helpful indexes
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.IsActive);
                b.HasIndex(x => x.ExpiresAt);
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
                 .OnDelete(DeleteBehavior.Cascade); // remove notifications if user deleted

                // Useful indexes for inbox queries
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => new { x.UserId, x.IsRead });
                b.HasIndex(x => x.Priority);
                b.HasIndex(x => x.IsDeleted);
            });

            modelBuilder.Entity<HealthMetric>(b =>
            {
                b.HasKey(x => x.HealthMetricId);

                b.Property(x => x.MetricCode).HasMaxLength(50).IsRequired();
                b.Property(x => x.Unit).HasMaxLength(20).IsRequired();
                b.Property(x => x.Value).HasPrecision(10, 2);     // decimal(10,2)
                b.Property(x => x.Notes).HasMaxLength(300);

                b.HasOne(x => x.User)
                 .WithMany(u => u.HealthMetrics)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade);               // tie to patient 

                // Helpful indexes
                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.MetricCode);
                b.HasIndex(x => x.MeasuredAt);
                b.HasIndex(x => x.IsDeleted);
            });

            modelBuilder.Entity<OtpVerification>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Map the hash column, not OtpCode, for length
                entity.Property(e => e.OtpCodeHash)
                    .HasMaxLength(256)
                    .IsRequired(false);

                // Do NOT set HasMaxLength on int OtpCode; remove previous MaxLength(10) line

                entity.Property(e => e.Purpose)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Expiry).IsRequired();

                entity.Property(e => e.Used).HasDefaultValue(false);

                entity.Property(e => e.Attempts).HasDefaultValue(0);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.Purpose, e.Expiry, e.Used });
            });

            // BloodType
            modelBuilder.Entity<BloodType>(b =>
            {
                b.ToTable("BloodType", schema: "master");
                b.HasKey(x => x.BloodTypeId);
                b.Property(x => x.Name).HasMaxLength(5).IsRequired();
                b.HasIndex(x => x.Name).IsUnique();

                b.Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()").ValueGeneratedOnAdd();
                b.Property(x => x.CreatedBy).HasDefaultValue(1);

                b.HasData(BloodTypeData.GetSeed());
            });

            // Consultation
            modelBuilder.Entity<Consultation>(b =>
            {
                b.ToTable("Consultation");
                b.HasKey(x => x.ConsultationId);

                b.HasIndex(x => x.AppointmentId).IsUnique();

                b.HasOne(x => x.Appointment)
                    .WithOne(a => a.Consultation)
                    .HasForeignKey<Consultation>(c => c.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Doctor)
                    .WithMany()
                    .HasForeignKey(x => x.DoctorId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(x => x.Patient)
                    .WithMany()
                    .HasForeignKey(x => x.PatientId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasMany(x => x.Files)
                    .WithOne(f => f.Consultation)
                    .HasForeignKey(f => f.ConsultationId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(x => x.Prescriptions)
                    .WithOne(p => p.Consultation)
                    .HasForeignKey(p => p.ConsultationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // JSON conversion for Dictionary<string, decimal>
                b.Property(x => x.HealthValues)
                    .HasColumnType("nvarchar(max)")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = false
                        }),
                        v => JsonSerializer.Deserialize<Dictionary<string, decimal>>(v, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })!
                    );

                b.Property(x => x.TrendSummary)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Prescription>(b =>
            {
                b.ToTable("Prescriptions");
                b.HasKey(x => x.PrescriptionId);
                b.Property(x => x.Notes).HasMaxLength(4000);
                b.Property(x => x.CreatedAt).IsRequired();
                b.HasIndex(x => x.ConsultationId);

                b.HasOne(x => x.Consultation)
                 .WithMany()
                 .HasForeignKey(x => x.ConsultationId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // PrescriptionItem
            modelBuilder.Entity<PrescriptionItem>(b =>
            {
                b.ToTable("PrescriptionItems");
                b.HasKey(x => x.PrescriptionItemId);
                b.Property(x => x.Medicine).IsRequired().HasMaxLength(500);
                b.Property(x => x.Strength).HasMaxLength(200);
                b.Property(x => x.Dose).HasMaxLength(200);
                b.Property(x => x.Frequency).HasMaxLength(200);
                b.Property(x => x.Route).HasMaxLength(200);
                b.HasIndex(x => x.PrescriptionId);

                b.HasOne(x => x.Prescription)
                 .WithMany(p => p.Items)
                 .HasForeignKey(x => x.PrescriptionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // MedicalRecord
            modelBuilder.Entity<MedicalRecord>(b =>
            {
                b.ToTable("MedicalRecords");
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
                b.HasOne<RecordType>()
        .WithMany(rt => rt.MedicalRecords)
        .HasForeignKey(x => x.RecordTypeId)
        .OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(x => x.UserId);
                b.HasIndex(x => x.RecordDate);
                b.HasIndex(x => x.IsDeleted);
            });

            modelBuilder.Entity<RecordType>(b =>
            {
                b.ToTable("RecordType", schema: "master");
                b.HasKey(x => x.RecordTypeId);

                b.Property(x => x.Name)
                    .HasMaxLength(50)
                    .IsRequired();

               
                b.HasData(RecordTypeData.GetSeed());
            });


        }
    }
}
