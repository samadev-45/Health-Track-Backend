using System;

namespace Health.Application.DTOs
{
    public class ConsultationResponseDto
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public int UserId { get; set; }

        public string? DoctorNotes { get; set; }
        public string HealthValuesJson { get; set; } = "{}";
        public string? TrendSummary { get; set; } // optional for phase 2
        public DateTime CreatedOn { get; set; }
    }
}
