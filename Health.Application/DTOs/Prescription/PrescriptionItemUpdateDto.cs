namespace Health.Application.DTOs
{
    public class PrescriptionItemUpdateDto
    {
        public string? Medicine { get; set; }
        public string? Strength { get; set; }
        public string? Dose { get; set; }
        public string? Frequency { get; set; }
        public int? DurationDays { get; set; }
        public string? Route { get; set; }
        public string? Notes { get; set; }
    }
}
