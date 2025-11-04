using System;

namespace Health.Application.DTOs.Appointments
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }

        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Status { get; set; } = string.Empty;

        public string? Hospital { get; set; }
        public string? Location { get; set; }
        public string? PatientNotes { get; set; }
        public string? DoctorNotes { get; set; }
    }
}
