using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs
{
    public class ConsultationDetailsDto
    {
        public int ConsultationId { get; set; }
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? Advice { get; set; }
        public string? DoctorNotes { get; set; }
        public string HealthValuesJson { get; set; } = "{}";
        public DateTime? FollowUpDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPrescriptionGenerated { get; set; }
        public int? PrescriptionId { get; set; }
        public DateTime? PrescriptionDate { get; set; }
        public List<PrescriptionItemDto> PrescriptionItems { get; set; } = new();
        public List<FileDto> Attachments { get; set; } = new();
    }
}
