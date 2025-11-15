using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.DTOs.Appointment
{
    public class RespondToAppointmentDto
    {
        public bool IsAccepted { get; set; }
        public string? Remarks { get; set; }
    }
}