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
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<CaretakerAccess> CaretakerAccesses { get; set; } = null!;

        //public DbSet<Appointment> Appointments { get; set; } = null!;
        //public DbSet<AppointmentHistory> AppointmentHistories { get; set; } = null!;

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
            //Refresh token
            modelBuilder.Entity<RefreshToken>(b =>
            {
                b.HasKey(x => x.TokenId);

                b.Property(x => x.Token)
                 .IsRequired();

                b.HasIndex(x => x.Token).IsUnique(); 

                b.HasOne(x => x.User)
                 .WithMany(u => u.RefreshTokens)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Cascade); 

                b.HasIndex(x => x.IsDeleted);
            });
            //CaretakerAccess
            modelBuilder.Entity<CaretakerAccess>(b =>
            {
                b.HasKey(x => x.AccessId);

                // Self-referencing relationships to Users (two FKs)
                b.HasOne(x => x.Patient)
                 .WithMany(u => u.Caretakers)
                 .HasForeignKey(x => x.PatientId)
                 .OnDelete(DeleteBehavior.Restrict); 

                b.HasOne(x => x.Caretaker)
                 .WithMany(u => u.PatientsUnderCare)
                 .HasForeignKey(x => x.CaretakerId)
                 .OnDelete(DeleteBehavior.Restrict); 

                // Ensure patient and caretaker are not the same user
                b.HasCheckConstraint("CK_CaretakerAccess_Patient_NE_Caretaker", "[PatientId] <> [CaretakerId]");

                // Helpful indexes
                b.HasIndex(x => x.PatientId);
                b.HasIndex(x => x.CaretakerId);
                b.HasIndex(x => new { x.PatientId, x.CaretakerId }).IsUnique(false); 
                b.HasIndex(x => x.IsActive);
                b.HasIndex(x => x.IsDeleted);

                // Enum storage as int
                b.Property(x => x.Relationship).HasConversion<int>(); 
                b.Property(x => x.AccessLevel).HasConversion<int>();  

                // Lengths
                b.Property(x => x.Notes).HasMaxLength(500);
            });

            modelBuilder.Entity<Appointment>(b =>
            {
                b.HasKey(x => x.AppointmentId);

                b.Property(x => x.Status).HasConversion<int>(); // enum to int [web:11]
                b.Property(x => x.PatientNotes).HasMaxLength(500);
                b.Property(x => x.DoctorNotes);
                b.Property(x => x.RejectionReason).HasMaxLength(500);
                b.Property(x => x.Hospital).HasMaxLength(200);
                b.Property(x => x.Location).HasMaxLength(300);

                // Two FKs to Users; use Restrict to prevent multiple cascade paths on self-ref FKs
                b.HasOne(x => x.Patient)
                 .WithMany(u => u.AppointmentsAsPatient)
                 .HasForeignKey(x => x.PatientId)
                 .OnDelete(DeleteBehavior.Restrict); // avoid cascade cycles [web:144][web:155][web:136]

                b.HasOne(x => x.Doctor)
                 .WithMany(u => u.AppointmentsAsDoctor)
                 .HasForeignKey(x => x.DoctorId)
                 .OnDelete(DeleteBehavior.Restrict); // symmetrical [web:144][web:155]

                b.HasIndex(x => x.PatientId);
                b.HasIndex(x => x.DoctorId);
                b.HasIndex(x => x.AppointmentDate);
                b.HasIndex(x => x.Status);
                b.HasIndex(x => x.IsDeleted);
            });

            //AppointmentHistory
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
        }


    }
}
