using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Medication
{
    public class MedicationDto
    {
        public int MedicationId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public decimal? DosageValue { get; set; }
        public int? UnitId { get; set; }
        public string? UnitName { get; set; }
        public string? Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? RemainingDays { get; set; }
        public string? Instructions { get; set; }
    }

}
