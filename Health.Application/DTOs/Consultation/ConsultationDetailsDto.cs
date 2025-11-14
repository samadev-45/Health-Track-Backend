using Health.Application.DTOs.File;
using Health.Application.DTOs.Prescription;

public class ConsultationDetailsDto
{
    public int ConsultationId { get; set; }
    public int AppointmentId { get; set; }

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = "";

    public int PatientId { get; set; }
    public string PatientName { get; set; } = "";

    public string? Diagnosis { get; set; }
    public string? Advice { get; set; }
    public string? DoctorNotes { get; set; }

    public Dictionary<string, decimal>? HealthValues { get; set; } = new();

    public string? TrendSummary { get; set; }  // NEW
    public string? HealthValuesJson { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public int Status { get; set; }
    public bool IsPrescriptionGenerated { get; set; }

    public int? PrescriptionId { get; set; }
    public DateTime? PrescriptionDate { get; set; }

    public List<PrescriptionItemDto> PrescriptionItems { get; set; } = new();
    public List<FileDto> Attachments { get; set; } = new();
}
