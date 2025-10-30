namespace Health.Application.DTOs
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = null!;
        public int Otp { get; set; }
        public string NewPassword { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
