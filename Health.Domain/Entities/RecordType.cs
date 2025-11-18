using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class RecordType : BaseEntity
    {
        public int RecordTypeId { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
