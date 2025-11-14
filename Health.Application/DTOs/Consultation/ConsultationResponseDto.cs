using System;

namespace Health.Application.DTOs.Consultation
{
    public class ConsultationResponseDto
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }

        // Participants
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }

        // Clinical data
        public string? ChiefComplaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? Advice { get; set; }
        public string? DoctorNotes { get; set; }
        public string? TrendSummary { get; set; }
        public Dictionary<string, decimal>? HealthValues { get; set; }


        // Follow-up & metadata
        public DateTime? FollowUpDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPrescriptionGenerated { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        // Optional (future use)
        

    }
}
