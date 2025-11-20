
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class HealthMetric : BaseEntity
    {
        public int HealthMetricId { get; set; }

        public int UserId { get; set; }                 // patient
        public int MetricTypeId { get; set; }
        public MetricType MetricType { get; set; } = null!;
        // e.g., "glucose", "sodium", "potassium", "temperature"
        public decimal Value { get; set; }              // numeric value
        
        public DateTime MeasuredAt { get; set; }

        public bool IsAbnormal { get; set; } = false;   // computed at insert/update
        public string? Notes { get; set; }

        public User User { get; set; } = null!;
    }
}
