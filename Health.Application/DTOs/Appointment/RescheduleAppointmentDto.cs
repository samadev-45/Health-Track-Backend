using System;

namespace Health.Application.DTOs.Appointment
{
    public class RescheduleAppointmentDto
    {
       
        public DateTime NewAppointmentDate { get; set; }    // date part
        public TimeSpan NewAppointmentTime { get; set; }    // time part
        public string? Reason { get; set; }
    }
}