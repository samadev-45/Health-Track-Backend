using Health.Domain.Common;
using System;

namespace Health.Domain.Entities
{
    public class Consultation : BaseEntity
    {
        public int ConsultationId { get; set; }

        public int AppointmentId { get; set; }      // FK to Appointment (1:1)
        public int DoctorId { get; set; }           // FK to User (Doctor)
        public int PatientId { get; set; }          // FK to User (Patient)

        public string? ChiefComplaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? Advice { get; set; }
        public string? DoctorNotes { get; set; }

        public string HealthValuesJson { get; set; } = "{}";
        public DateTime? FollowUpDate { get; set; }

        public ConsultationStatus Status { get; set; } = ConsultationStatus.Draft;
        public bool IsPrescriptionGenerated { get; set; } = false;

        // Navigation
        public Appointment? Appointment { get; set; }

        public User? Doctor { get; set; }
        public User? Patient { get; set; }
        public ICollection<FileStorage> Files { get; set; }=new List<FileStorage>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
