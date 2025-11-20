using Health.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Health.Infrastructure.Data
{
    public static class MetricTypeData
    {
        private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static IEnumerable<MetricType> GetSeed()
        {
            return new List<MetricType>
            {
                new MetricType {
                    MetricTypeId = 1,
                    MetricCode = "bp_sys",
                    DisplayName = "Blood Pressure (Systolic)",
                    Unit = "mmHg",
                    MaxValue = 120,
                    MinValue = 80,
                    CreatedOn = SeedDate,
                    CreatedBy = 1
                },
                new MetricType {
                    MetricTypeId = 2,
                    MetricCode = "bp_dia",
                    DisplayName = "Blood Pressure (Diastolic)",
                    Unit = "mmHg",
                    MaxValue = 80,
                    MinValue = 60,
                    CreatedOn = SeedDate,
                    CreatedBy = 1
                },
                new MetricType {
                    MetricTypeId = 3,
                    MetricCode = "glucose_fast",
                    DisplayName = "Blood Glucose (Fasting)",
                    Unit = "mg/dL",
                    MaxValue = 100,
                    MinValue = 70,
                    CreatedOn = SeedDate,
                    CreatedBy = 1
                },
                new MetricType {
                    MetricTypeId = 4,
                    MetricCode = "glucose_pp",
                    DisplayName = "Blood Glucose (Postprandial)",
                    Unit = "mg/dL",
                    MaxValue = 140,
                    MinValue = 70,
                    CreatedOn = SeedDate,
                    CreatedBy = 1
                },
                new MetricType {
                    MetricTypeId = 5,
                    MetricCode = "spo2",
                    DisplayName = "Oxygen Saturation",
                    Unit = "%",
                    MinValue = 95,
                    MaxValue = null,
                    CreatedOn = SeedDate,
                    CreatedBy = 1
                },
                new MetricType {
                    MetricTypeId = 6,
                    MetricCode = "heart_rate",
                    DisplayName = "Heart Rate",
                    Unit = "bpm",
                    MaxValue = 100,
                    MinValue = 60,
                    CreatedOn = SeedDate,
                    CreatedBy = 1
                },
            };
        }
    }
}
