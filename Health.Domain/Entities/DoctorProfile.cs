using System;
using Health.Domain.Enums;

namespace Health.Domain.Entities
{
    public class DoctorProfile
    {
        public int DoctorProfileId { get; set; }
        public int UserId { get; set; }

        public string? Hospital { get; set; }
        public string? Location { get; set; }
        public string? ClinicName { get; set; }
        public string? About { get; set; }
        public int ExperienceYears { get; set; }

        // Availability
        public TimeSpan? AvailableFrom { get; set; }
        public TimeSpan? AvailableTo { get; set; }
        public AvailableDays AvailableDays { get; set; } // e.g. "Mon,Tue,Wed"

        public decimal? ConsultationFee { get; set; }

        // Navigation
        public User? Doctor { get; set; }
    }
}
