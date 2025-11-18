using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Medication
{
    public class MedicationScheduleItemDto
    {
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = null!;
        public DateTime ScheduledAt { get; set; }    // exact time for today
        public string Frequency { get; set; } = null!;
        public string? Instructions { get; set; }
    }

}
