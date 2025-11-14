using System.Collections.Generic;

namespace Health.Application.DTOs
{
    public class PrescriptionCreateDto
    {
        public string? Notes { get; set; }
        public List<PrescriptionItemCreateDto>? Items { get; set; } = new();
    }
}
