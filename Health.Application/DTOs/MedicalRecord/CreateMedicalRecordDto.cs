using Microsoft.AspNetCore.Http;

namespace Health.Application.DTOs.MedicalRecord
{
    public class CreateMedicalRecordDto
    {
        public int RecordTypeId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.UtcNow;
        public string? DoctorName { get; set; }
        public string? Hospital { get; set; }

        // File
        public string Base64Data { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = "application/octet-stream";
        public long FileSize { get; set; }
    }
}
