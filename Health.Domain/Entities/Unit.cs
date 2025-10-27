// Health.Domain/Entities/Unit.cs
using Health.Domain.Common;
using System.Collections.Generic;

namespace Health.Domain.Entities
{
    public class Unit : BaseEntity
    {
        public int UnitId { get; set; }

        // e.g., "mg", "ml", "tablet"
        public string UnitName { get; set; } = null!;

        
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();

        
        public ICollection<Medication> DoseRangeMedications { get; set; } = new List<Medication>();
    }
}
