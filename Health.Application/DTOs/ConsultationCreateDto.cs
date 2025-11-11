using System;

namespace Health.Application.DTOs
{
    public class ConsultationCreateDto
    {
        // The appointment to which this consultation belongs
        public int AppointmentId { get; set; }

        // Optional doctor notes or summary
        public string? DoctorNotes { get; set; }

        // Health metrics JSON payload (BP, sugar, etc.)
        public string HealthValuesJson { get; set; } = "{}";
    }
}
