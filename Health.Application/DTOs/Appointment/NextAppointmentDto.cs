using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Appointment
{
    public class NextAppointmentDto
    {
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; } = "";
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string? Location { get; set; }
        public string? Hospital { get; set; }
    }

}
