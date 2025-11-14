//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text.Json;

//namespace Health.Application.Common
//{
//    public class HealthMetricEngine
//    {
//        private Dictionary<string, decimal>? _normalRanges;

//        public HealthMetricEngine()
//        {
//            LoadRanges();
//        }

//        private void LoadRanges()
//        {
//            if (_normalRanges != null)
//                return;

//            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "NormalRanges.json");

//            if (!File.Exists(jsonPath))
//                throw new FileNotFoundException("NormalRanges.json missing.", jsonPath);

//            var json = File.ReadAllText(jsonPath);
//            _normalRanges = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json)
//                             ?? new Dictionary<string, decimal>();
//        }

//        private bool IsValueAbnormal(string key, decimal value)
//        {
//            if (!_normalRanges!.TryGetValue(key, out var limitValue))
//                return false;

//            if (key.EndsWith("_Max", StringComparison.OrdinalIgnoreCase))
//                return value > limitValue;

//            if (key.EndsWith("_Min", StringComparison.OrdinalIgnoreCase))
//                return value < limitValue;

//            return false;
//        }

//        public Dictionary<string, decimal> ParseHealthJson(string json)
//        {
//            if (string.IsNullOrWhiteSpace(json))
//                return new Dictionary<string, decimal>();

//            return JsonSerializer.Deserialize<Dictionary<string, decimal>>(json)
//                   ?? new Dictionary<string, decimal>();
//        }

//        public List<string> DetectAbnormalMetrics(string healthJson)
//        {
//            var values = ParseHealthJson(healthJson);
//            var abnormal = new List<string>();

//            foreach (var entry in values)
//            {
//                if (IsValueAbnormal(entry.Key, entry.Value))
//                    abnormal.Add(entry.Key);
//            }

//            return abnormal;
//        }

//        public string BuildTrendSummary(Dictionary<string, decimal>? values)
//        {
//            if (values == null || values.Count == 0)
//                return "No health metrics provided.";

//            var abnormal = values
//                .Where(v => IsValueAbnormal(v.Key, v.Value))
//                .Select(v => v.Key)
//                .ToList();

//            if (!abnormal.Any())
//                return "All vital signs are within normal range.";

//            return "Abnormal metrics: " + string.Join(", ", abnormal);
//        }
//    }
//}

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

        // Accept a dictionary of numeric values (from consultation.HealthValues)
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
