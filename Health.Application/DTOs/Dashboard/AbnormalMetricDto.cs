using System;

namespace Health.Application.DTOs.Dashboard
{
    public class AbnormalMetricDto
    {
        public int MetricTypeId { get; set; }
        public string MetricCode { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal Value { get; set; }
        public DateTime MeasuredAt { get; set; }
        public string Status { get; set; } = null!;   // "High" / "Low"
    }
}
