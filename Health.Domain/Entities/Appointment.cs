// Health.Domain/Entities/Appointment.cs
using Health.Domain.Common;
using Health.Domain.Enums;

namespace Health.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public int AppointmentId { get; set; }

        // Relations
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        // Details
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        // Notes & reasons
        public string? PatientNotes { get; set; }
        public string? DoctorNotes { get; set; }
        public string? RejectionReason { get; set; }

        // Workflow integration
        public DateTime? FollowUpDate { get; set; }

        // Location (optional)
        public string? Hospital { get; set; }
        public string? Location { get; set; }

        // Navigations
        public User Patient { get; set; } = null!;
        public User Doctor { get; set; } = null!;
        public ICollection<AppointmentHistory> History { get; set; } = new List<AppointmentHistory>();
    }
}
