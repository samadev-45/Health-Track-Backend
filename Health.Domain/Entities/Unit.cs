
using Health.Domain.Common;
using System.Collections.Generic;

namespace Health.Domain.Entities
{
    public class Unit : BaseEntity
    {
        public int UnitId { get; set; }

        //  "mg", "ml", "tablet"
        public string UnitName { get; set; } = null!;

        
        public string? Description { get; set; }

        
        public ICollection<Medication> Medications { get; set; } = new List<Medication>();

        
        public ICollection<Medication> DoseRangeMedications { get; set; } = new List<Medication>();
    }
}
