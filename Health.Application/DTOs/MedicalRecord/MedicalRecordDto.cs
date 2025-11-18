using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.MedicalRecord
{
    public class MedicalRecordDto
    {
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public int RecordTypeId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime RecordDate { get; set; }
        public string? DoctorName { get; set; }
        public string? Hospital { get; set; }
        public int? FileStorageId { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public long? FileSize { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

