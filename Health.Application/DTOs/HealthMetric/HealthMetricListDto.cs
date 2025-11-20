using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.HealthMetric
{
    public class HealthMetricListDto
    {
        public int HealthMetricId { get; set; }
        public int MetricTypeId { get; set; }
        public string MetricCode { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal Value { get; set; }
        public DateTime MeasuredAt { get; set; }
        public bool IsAbnormal { get; set; }
    }
}

