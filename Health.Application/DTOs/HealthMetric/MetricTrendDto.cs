namespace Health.Application.DTOs.HealthMetric
{
    public class MetricTrendDto
    {
        public int MetricTypeId { get; set; }
        public string MetricCode { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Unit { get; set; } = null!;

        public List<TrendPoint> Points { get; set; } = new();

        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }

        public bool HasAbnormalReadings { get; set; }
    }

    public class TrendPoint
    {
        public DateTime MeasuredAt { get; set; }
        public decimal Value { get; set; }
        public bool IsAbnormal { get; set; }
    }
}
