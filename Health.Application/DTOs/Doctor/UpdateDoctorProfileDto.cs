using Health.Domain.Enums;
using System;

namespace Health.Application.DTOs.Doctor
{
    public class UpdateDoctorProfileDto
    {
        public string? Hospital { get; set; }
        public string? Location { get; set; }
        public string? ClinicName { get; set; }
        public string? About { get; set; }

        public int ExperienceYears { get; set; }
        public decimal? ConsultationFee { get; set; }

        public TimeSpan? AvailableFrom { get; set; }
        public TimeSpan? AvailableTo { get; set; }

        public AvailableDays AvailableDays { get; set; }  // e.g. "Mon,Tue,Wed"
    }
}
