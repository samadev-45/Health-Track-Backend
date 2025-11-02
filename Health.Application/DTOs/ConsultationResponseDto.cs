using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs
{
    public class ConsultationResponseDto
    {
        public int ConsultationId { get; set; }
        public string TrendSummary { get; set; } = ""; // e.g. "↑ sugar, ↓ weight"
        public string HealthValuesJson { get; set; } = "{}";
    }
}
