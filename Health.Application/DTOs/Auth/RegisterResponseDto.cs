namespace Health.Application.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsEmailVerified { get; set; }
        public string Status { get; set; } = "Pending";
        public string Message { get; set; } = "Registration successful.Waiting for admin approval";
    }
}
