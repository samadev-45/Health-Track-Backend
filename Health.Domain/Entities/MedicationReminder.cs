
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class MedicationReminder : BaseEntity
    {
        public int ReminderId { get; set; }

        public int MedicationId { get; set; }           // parent medication
        public DateTime RemindAt { get; set; }          // UTC due time
        public bool IsSent { get; set; } = false;       // mark when dispatched
        public DateTime? SentAt { get; set; }           // when sent (if sent)
        public string? Channel { get; set; }            // "SMS", "Email", "Push"
        public string? Notes { get; set; }              // optional user note

        public Medication Medication { get; set; } = null!;
    }
}
