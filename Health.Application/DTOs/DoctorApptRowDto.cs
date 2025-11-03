namespace Health.Application.DTOs
{
    public class DoctorApptRowDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public int Status { get; set; }
        public string? PatientNotes { get; set; }
        public string? DoctorNotes { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string? Hospital { get; set; }
        public string? Location { get; set; }
    }
}
