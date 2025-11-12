using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs
{
    public class PrescriptionItemDto
    {
        public int PrescriptionItemId { get; set; }
        public int PrescriptionId { get; set; }
        public string Medicine { get; set; } = string.Empty;
        public string? Strength { get; set; }
        public string? Dose { get; set; }
        public string? Frequency { get; set; }
        public int? DurationDays { get; set; }
        public string? Route { get; set; }
        public string? Notes { get; set; }
    }

}
