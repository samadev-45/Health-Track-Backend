using System;

namespace Health.Application.DTOs.Appointment
{
    public class RescheduleAppointmentDto
    {
        public int AppointmentId { get; set; }
        public DateTime NewAppointmentDate { get; set; }    // date part
        public TimeSpan NewAppointmentTime { get; set; }    // time part
        public string? Reason { get; set; }
    }
}