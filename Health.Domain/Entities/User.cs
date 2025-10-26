using Health.Domain.Common;
using Health.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Health.Domain.Entities
{
    public class User : BaseEntity
    {
        public int UserId { get; set; }

        // Basic
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? PhoneNumber { get; set; }

        // Profile
        public DateTime? DateOfBirth { get; set; }
        public GenderType? Gender { get; set; }
        public string? BloodType { get; set; }
        public string? Address { get; set; }

        // Emergency profile
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? MedicalConditions { get; set; }
        public string? Allergies { get; set; }
        public string? CurrentMedications { get; set; }

        // Account
        public RoleType Role { get; set; } = RoleType.Patient;
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;

        // Doctor-only (optional)
        public int? SpecialtyId { get; set; }
        public string? LicenseNumber { get; set; }
        public bool IsVerified { get; set; } = false;

        // Navigations (filled as you add other entities)
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<CaretakerAccess> Caretakers { get; set; } = new List<CaretakerAccess>();
        public ICollection<CaretakerAccess> PatientsUnderCare { get; set; } = new List<CaretakerAccess>();
        //public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Appointment> AppointmentsAsPatient { get; set; } = new List<Appointment>();
        public ICollection<Appointment> AppointmentsAsDoctor { get; set; } = new List<Appointment>();
        //public ICollection<Medication> Medications { get; set; } = new List<Medication>();
        //public ICollection<HealthMetric> HealthMetrics { get; set; } = new List<HealthMetric>();
        //public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        //public ICollection<ShareableLink> ShareableLinks { get; set; } = new List<ShareableLink>();
        //public ICollection<FileStorage> UploadedFiles { get; set; } = new List<FileStorage>();
    }
}
