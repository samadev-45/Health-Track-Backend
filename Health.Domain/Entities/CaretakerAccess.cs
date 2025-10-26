// Health.Domain/Entities/CaretakerAccess.cs
using Health.Domain.Common;
using Health.Domain.Enums;

namespace Health.Domain.Entities
{
    public class CaretakerAccess : BaseEntity
    {
        public int AccessId { get; set; }

        // Relations (self-reference to Users)
        public int PatientId { get; set; }
        public int CaretakerId { get; set; }

        // Details
        public RelationshipType Relationship { get; set; }  // Parent, Spouse, etc.
        public AccessLevel AccessLevel { get; set; } = AccessLevel.ViewOnly;
        public bool IsActive { get; set; } = true;
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Notes { get; set; }

        // Navigations
        public User Patient { get; set; } = null!;
        public User Caretaker { get; set; } = null!;
    }
}
        