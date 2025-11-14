namespace Health.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string? RefreshToken { get; set; } // optional
        
    }
}
