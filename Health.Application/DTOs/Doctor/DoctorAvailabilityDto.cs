using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Doctor
{
    public class DoctorAvailabilityDto
    {
        public string Hospital { get; set; }
        public string Location { get; set; }
        public IEnumerable<string> Slots { get; set; }
    }

}
