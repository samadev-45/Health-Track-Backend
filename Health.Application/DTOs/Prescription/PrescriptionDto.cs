using Health.Application.DTOs.Prescription;
using System;
using System.Collections.Generic;

namespace Health.Application.DTOs
{
    public class PrescriptionDto
    {
        public int PrescriptionId { get; set; }
        public int ConsultationId { get; set; }
        public int CreatedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PrescriptionItemDto> Items { get; set; } = new();
    }
}
