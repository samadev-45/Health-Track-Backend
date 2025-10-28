﻿namespace Health.Application.DTOs
{
    public class RegisterResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsEmailVerified { get; set; }

        public string Message { get; set; } = "Registration successful";
    }
}
