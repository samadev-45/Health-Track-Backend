// Health.Application.DTOs.Appointments/CancelAppointmentDto.cs
using System.ComponentModel.DataAnnotations;

namespace Health.Application.DTOs
{
    public class CancelAppointmentDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;
    }
}
