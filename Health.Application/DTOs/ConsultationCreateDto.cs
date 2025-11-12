using System;

namespace Health.Application.DTOs
{
    public class ConsultationCreateDto
    {
        /// <summary>
        /// Appointment to which this consultation belongs.
        /// </summary>
        public int AppointmentId { get; set; }

        /// <summary>
        /// Patient’s main complaint or reason for visit.
        /// </summary>
        public string? ChiefComplaint { get; set; }

        /// <summary>
        /// Doctor’s diagnosis text.
        /// </summary>
        public string? Diagnosis { get; set; }

        /// <summary>
        /// Doctor’s advice or treatment recommendation.
        /// </summary>
        public string? Advice { get; set; }

        /// <summary>
        /// Any private notes from the doctor.
        /// </summary>
        public string? DoctorNotes { get; set; }

        /// <summary>
        /// Health metric JSON payload (BP, sugar, etc.).
        /// </summary>
        public string HealthValuesJson { get; set; } = "{}";

        /// <summary>
        /// Optional follow-up appointment date.
        /// </summary>
        public DateTime? FollowUpDate { get; set; }
    }
}
