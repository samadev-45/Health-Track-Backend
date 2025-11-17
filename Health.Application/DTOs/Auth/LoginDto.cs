using System.ComponentModel.DataAnnotations;

namespace Health.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        
        public string Password { get; set; } = null!;
    }
}
