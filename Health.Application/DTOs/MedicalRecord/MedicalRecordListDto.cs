using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.MedicalRecord
{
    public class MedicalRecordListDto
    {
        public int RecordId { get; set; }
        public string Title { get; set; } = null!;
        public int RecordTypeId { get; set; }
        public DateTime RecordDate { get; set; }
        public string? DoctorName { get; set; }
        public string? Hospital { get; set; }
        public int? FileStorageId { get; set; }
    }
}

