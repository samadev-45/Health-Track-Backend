
using Health.Domain.Common;

namespace Health.Domain.Entities
{
    public class ShareableLink : BaseEntity
    {
        public int ShareableLinkId { get; set; }

        public int UserId { get; set; }                 // owner/patient
        public string Token { get; set; } = null!;      // unique, random
        public DateTime ExpiresAt { get; set; }         // UTC expiry
        public bool IsActive { get; set; } = true;      // can be toggled off
        public DateTime? RevokedAt { get; set; }        // explicit revoke time

        
        public bool IncludeMedicalRecords { get; set; } = true;
        public bool IncludeAppointments { get; set; } = false;
        

        // Audit
        public int ViewCount { get; set; } = 0;
        public DateTime? LastAccessedAt { get; set; }

        public User User { get; set; } = null!;
    }
}
