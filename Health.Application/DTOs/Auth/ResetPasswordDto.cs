namespace Health.Application.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; }
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
