using Health.Domain.Common;
using System;

namespace Health.Domain.Entities
{
    public class Consultation : BaseEntity
    {
        public int ConsultationId { get; set; }
        public int UserId { get; set; }                 // patient id
        public string HealthValuesJson { get; set; } = "{}"; // JSON payload as string
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // navigations
        public User? User { get; set; }
    }
}
