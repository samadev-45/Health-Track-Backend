using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Health.Infrastructure.Services
{
    public class NormalRangeService
    {
        private Dictionary<string, decimal>? _cachedRanges;

        public Dictionary<string, decimal> GetNormalRanges()
        {
            if (_cachedRanges != null)
                return _cachedRanges;

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "NormalRanges.json");
            var json = File.ReadAllText(jsonPath);
            _cachedRanges = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json)!;

            return _cachedRanges;
        }

        public bool IsAbnormal(string recordType, decimal value)
        {
            var ranges = GetNormalRanges();

            // Try to find the exact key (e.g., "BP_Systolic_Max")
            foreach (var kvp in ranges)
            {
                if (recordType.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    // Detect whether this key represents a "Max" or "Min" limit
                    if (kvp.Key.Contains("Max", StringComparison.OrdinalIgnoreCase))
                        return value > kvp.Value;

                    if (kvp.Key.Contains("Min", StringComparison.OrdinalIgnoreCase))
                        return value < kvp.Value;
                }
            }

            
            return false;
        }

        public IEnumerable<string> GetAbnormalMetrics(Dictionary<string, decimal> consultationValues)
        {
            var ranges = GetNormalRanges();
            var abnormalKeys = new List<string>();

            foreach (var entry in consultationValues)
            {
                if (IsAbnormal(entry.Key, entry.Value))
                    abnormalKeys.Add(entry.Key);
            }

            return abnormalKeys;
        }
    } 
}
