
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class MedicalRecord : BaseEntity
    {
        public int RecordId { get; set; }

        public int UserId { get; set; }                       // patient
        public int RecordTypeId { get; set; }                 // FK to master later

        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime RecordDate { get; set; }

        public string? DoctorName { get; set; }
        public string? Hospital { get; set; }

        public int? FileStorageId { get; set; }               
        public FileStorage? File { get; set; }

        public User User { get; set; } = null!;
        public RecordType? RecordType { get; set; }
    }
}
