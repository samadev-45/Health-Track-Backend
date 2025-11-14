
namespace Health.Application.DTOs
{
    public class PrescriptionItemCreateDto
    {
        public string Medicine { get; set; } = string.Empty;
        public string? Strength { get; set; }
        public string? Dose { get; set; }
        public string? Frequency { get; set; }
        public int? DurationDays { get; set; }
        public string? Route { get; set; }
        public string? Notes { get; set; }
    }
}
