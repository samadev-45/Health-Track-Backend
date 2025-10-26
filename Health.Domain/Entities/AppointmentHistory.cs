// Health.Domain/Entities/AppointmentHistory.cs
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class AppointmentHistory : BaseEntity
    {
        public int HistoryId { get; set; }
        public int AppointmentId { get; set; }

        public string Action { get; set; } = null!; // Created, Rescheduled, DoctorChanged, Cancelled, StatusUpdated
        public int? OldDoctorId { get; set; }
        public int? NewDoctorId { get; set; }
        public DateTime? OldAppointmentDate { get; set; }
        public DateTime? NewAppointmentDate { get; set; }

        public int ChangedByUserId { get; set; }
        public string? Reason { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        public Appointment Appointment { get; set; } = null!;
        public User ChangedByUser { get; set; } = null!;
    }
}
