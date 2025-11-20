
// Health.Application/Common/HealthMetricEngine.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Health.Application.Common
{
    public class HealthMetricEngine
    {
        private readonly Dictionary<string, decimal> _normalRanges;

        public HealthMetricEngine(Dictionary<string, decimal> normalRanges)
        {
            _normalRanges = normalRanges ?? throw new ArgumentNullException(nameof(normalRanges));
        }

       
        public string BuildTrendSummary(Dictionary<string, decimal>? values)
        {
            if (values == null || values.Count == 0)
                return "No metrics available";

            var abnormal = new List<string>();

            foreach (var kv in values)
            {
                var key = kv.Key;
                var value = kv.Value;

                // check max
                var maxKey = key.EndsWith("_Max", StringComparison.OrdinalIgnoreCase) ? key : key + "_Max";
                if (_normalRanges.TryGetValue(maxKey, out var maxLimit) && value > maxLimit)
                {
                    abnormal.Add($"{key}: High");
                    continue;
                }

                // check min
                var minKey = key.EndsWith("_Min", StringComparison.OrdinalIgnoreCase) ? key : key + "_Min";
                if (_normalRanges.TryGetValue(minKey, out var minLimit) && value < minLimit)
                {
                    abnormal.Add($"{key}: Low");
                }
            }

            return abnormal.Count == 0 ? "Normal" : string.Join("; ", abnormal);
        }
    }
}
