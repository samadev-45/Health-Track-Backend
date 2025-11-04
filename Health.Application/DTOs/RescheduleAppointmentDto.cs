// Health.Application.DTOs.Appointments/RescheduleAppointmentDto.cs
using System;

namespace Health.Application.DTOs
{
    public class RescheduleAppointmentDto
    {
        public int AppointmentId { get; set; }
        public DateTime NewAppointmentDate { get; set; }    // date part
        public TimeSpan NewAppointmentTime { get; set; }    // time part
        public string? Reason { get; set; }
    }
}
