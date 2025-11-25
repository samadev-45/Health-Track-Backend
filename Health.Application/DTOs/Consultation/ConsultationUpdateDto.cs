using System;

namespace Health.Application.DTOs.Consultation
{
    public class ConsultationUpdateDto
    {
        public string? ChiefComplaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? Advice { get; set; }
        public string? DoctorNotes { get; set; }
        public Dictionary<string, decimal>? HealthValues { get; set; }
        public string? TrendSummary { get; set; }
        public DateTime? FollowUpDate { get; set; }
    }
}
