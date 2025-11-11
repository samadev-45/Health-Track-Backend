using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class PrescriptionItem : BaseEntity
    {
        public int PrescriptionItemId { get; set; }
        public int PrescriptionId { get; set; }

        public string Medicine { get; set; } = null!;
        public string? Strength { get; set; }
        public string? Dose { get; set; }          // e.g., "1 tablet"
        public string? Frequency { get; set; }     // e.g., "3 times/day"
        public int? DurationDays { get; set; }
        public string? Route { get; set; }         // e.g., "oral"
        public string? Notes { get; set; }

        public Prescription? Prescription { get; set; }
    }
}