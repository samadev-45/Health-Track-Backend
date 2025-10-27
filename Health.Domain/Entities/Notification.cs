// Health.Domain/Entities/Notification.cs
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }                 // recipient
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Category { get; set; }           // Appointment, Medication, System
        public string? ActionUrl { get; set; }          // deep link
        public int Priority { get; set; } = 0;          // 0=normal, 1=high

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        public User User { get; set; } = null!;
    }
}
