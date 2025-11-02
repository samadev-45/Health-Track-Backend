using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Domain.Entities
{
    public class NormalRange
    {
        public decimal BP_Systolic_Max { get; set; }
        public decimal BP_Diastolic_Max { get; set; }
        public decimal HeartRate_Max { get; set; }
        public decimal RespRate_Max { get; set; }
        public decimal Temperature_Max { get; set; }
        public decimal OxygenSaturation_Min { get; set; }
        public decimal BloodGlucoseFasting_Max { get; set; }
        public decimal BloodGlucosePostprandial_Max { get; set; }
    }
}

