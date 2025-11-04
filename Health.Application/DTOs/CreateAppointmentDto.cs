using System;
using System.ComponentModel.DataAnnotations;

namespace Health.Application.DTOs.Appointments
{
    public class CreateAppointmentDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan AppointmentTime { get; set; }

        public string? PatientNotes { get; set; }

        public string? Hospital { get; set; }

        public string? Location { get; set; }
    }
}
