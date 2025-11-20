using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.HealthMetric
{
    public class CreateHealthMetricDto
    {
        public int MetricTypeId { get; set; }

        public decimal Value { get; set; }
        
        public DateTime? MeasuredAt { get; set; }        
        public string? Notes { get; set; }
    }
}
