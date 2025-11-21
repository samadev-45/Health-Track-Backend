using System;
using System.Collections.Generic;

namespace Health.Application.DTOs.Dashboard
{
    public class LatestVitalsDto
    {
        public IEnumerable<VitalReadingDto> Readings { get; set; }
            = new List<VitalReadingDto>();
    }

    public class VitalReadingDto
    {
        public string MetricCode { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal Value { get; set; }
        public DateTime MeasuredAt { get; set; }
    }
}
