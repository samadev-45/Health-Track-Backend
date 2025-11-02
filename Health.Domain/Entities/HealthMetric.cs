
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class HealthMetric : BaseEntity
    {
        public int HealthMetricId { get; set; }

        public int UserId { get; set; }                 // patient
        public string MetricCode { get; set; } = null!; // e.g., "glucose", "sodium", "potassium", "temperature"
        public decimal Value { get; set; }              // numeric value
        public string Unit { get; set; } = null!;       // e.g., "mg/dL", "mmol/L", "°C"
        public DateTime MeasuredAt { get; set; }

        public bool IsAbnormal { get; set; } = false;   // computed at insert/update
        public string? Notes { get; set; }

        public User User { get; set; } = null!;
    }
}
