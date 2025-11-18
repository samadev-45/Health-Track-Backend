using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Medication
{
    public class MedicationListDto
    {
        public int MedicationId { get; set; }
        public string Name { get; set; } = null!;
        public string? Frequency { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UnitId { get; set; }
    }

}
