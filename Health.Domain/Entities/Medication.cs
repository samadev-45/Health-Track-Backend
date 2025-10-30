
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class Medication : BaseEntity
    {
        public int MedicationId { get; set; }

        public int UserId { get; set; }
        public string Name { get; set; } = null!;

        // Structured dosage
        public decimal? DosageValue { get; set; }     // supports 7.5 etc.
        public int UnitId { get; set; }               // FK -> Unit (UCUM code)

        // Optional dose range
        public decimal? DoseRangeLow { get; set; }
        public decimal? DoseRangeHigh { get; set; }
        public int? DoseRangeUnitId { get; set; }     // often same as UnitId

        // Schedule summary (can normalize later)
        public string? Frequency { get; set; }        // e.g., "2 times/day"
        public string? Instructions { get; set; }     // e.g., "after food"

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Navigations
        public User User { get; set; } = null!;
        public Unit Unit { get; set; } = null!;
        public Unit? DoseRangeUnit { get; set; }
        public ICollection<MedicationReminder> Reminders { get; set; } = new List<MedicationReminder>();
    }
}
