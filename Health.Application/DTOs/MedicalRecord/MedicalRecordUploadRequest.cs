using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.MedicalRecord
{
    public class MedicalRecordUploadRequest
    {
        public int RecordTypeId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? RecordDate { get; set; }
        public string? DoctorName { get; set; }
        public string? Hospital { get; set; }

        public IFormFile File { get; set; } = null!;
    }
}
