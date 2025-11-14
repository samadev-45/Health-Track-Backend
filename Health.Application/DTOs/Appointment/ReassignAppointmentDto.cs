// Health.Application.DTOs.Appointments/ReassignAppointmentDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Health.Application.DTOs.Appointment
{
    public class ReassignAppointmentDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int NewDoctorId { get; set; }

        [Required]
        public DateTime NewAppointmentDate { get; set; }   // date part (DateTime accepted)

        [Required]
        public TimeSpan NewAppointmentTime { get; set; }   // time part

        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
