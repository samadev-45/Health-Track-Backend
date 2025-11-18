using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Medication
{
    public class MedicationCreateDto
    {
        public string Name { get; set; } = null!;
        public decimal? DosageValue { get; set; }
        public int? UnitId { get; set; }
        public string? Frequency { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public decimal? DoseRangeLow { get; set; }
        public decimal? DoseRangeHigh { get; set; }
        public int? DoseRangeUnitId { get; set; }
        public string? Instructions { get; set; }
    }

}
