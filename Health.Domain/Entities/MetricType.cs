using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class MetricType:BaseEntity
    {
        public int MetricTypeId { get; set; }

        public string MetricCode { get; set; } = null!;  // glucose, bp_sys, etc.
        public string DisplayName { get; set; } = null!; // "Blood Pressure (Systolic)"
        public string Unit { get; set; } = null!;        // mg/dL, mmHg…

        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<HealthMetric> HealthMetrics { get; set; } = new List<HealthMetric>();
    }

}
