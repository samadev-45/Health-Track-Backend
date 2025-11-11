using Health.Domain.Common;
using System;
using System.Collections.Generic;

namespace Health.Domain.Entities
{
    public class Prescription : BaseEntity
    {
        public int PrescriptionId { get; set; }
        public int ConsultationId { get; set; }
        public int CreatedByUserId { get; set; } // doctor who created

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Consultation? Consultation { get; set; }
        public ICollection<PrescriptionItem>? Items { get; set; }
    }
}