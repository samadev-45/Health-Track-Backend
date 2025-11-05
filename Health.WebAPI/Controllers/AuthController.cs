using Health.Application.DTOs;
using Health.Application.Interfaces;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Health.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Health.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;
        private readonly HealthDbContext _context;

        public AuthController(
            IAuthService authService,
            IOtpService otpService,
            HealthDbContext context)
        {
            _authService = authService;
            _otpService = otpService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.RegisterAsync(registerDto);

            
            if (response.Success && response.Data is not null)
            {
                
                if (string.IsNullOrWhiteSpace((response.Data as RegisterResponseDto)!.Status))
                {
                    var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == registerDto.Email);
                    if (user is not null)
                        (response.Data as RegisterResponseDto)!.Status = user.Status.ToString();
                }
            }

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(loginDto);
            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }

        [HttpPost("request-email-Otp")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            try
            {
                await _otpService.GenerateAndSendOtpAsync(dto.Email, "ResetPassword");
                return Ok(new { message = "OTP sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to send OTP: {ex.Message}" });
            }
        }

        [HttpPost("Verify-email-Otp")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest(new { message = "User not found." });

            try
            {
                // dto.Otp is string; service will parse it to int internally
                await _otpService.ResetPasswordAsync(user.UserId, dto.Otp, dto.NewPassword, dto.ConfirmPassword);
                return Ok(new { message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Admin endpoints

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users/pending")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u => u.Status == AccountStatus.Pending && !u.IsDeleted)
                .Select(u => new
                {
                    u.UserId,
                    u.FullName,
                    u.Email,
                    Status = u.Status.ToString()
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/users/{id:int}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && !u.IsDeleted);
            if (user is null)
                return NotFound(new { message = "User not found." });

            var previous = user.Status;

            switch (user.Status)
            {
                case AccountStatus.Pending:
                    user.Status = AccountStatus.Approved;
                    break;
                case AccountStatus.Approved:
                    user.Status = AccountStatus.Rejected;
                    break;
                case AccountStatus.Rejected:
                    user.Status = AccountStatus.Approved;
                    break;
                default:
                    return BadRequest(new { message = $"Unsupported status: {user.Status}" });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                user.UserId,
                user.Email,
                previousStatus = previous.ToString(),
                currentStatus = user.Status.ToString(),
                message = $"Status changed from {previous} to {user.Status}."
            });
        }


        [HttpPost("caretaker/request-email-otp")]
        public async Task<IActionResult> RequestCaretakerEmailOtp([FromBody] CaretakerEmailOtpRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email is required." });

            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Role == RoleType.CareTaker && !u.IsDeleted);

            if (user is null)
            {
                user = new User
                {
                    FullName = string.IsNullOrWhiteSpace(dto.FullName) ? "Caretaker" : dto.FullName!.Trim(),
                    Email = email,
                    Role = RoleType.CareTaker,
                    Status = AccountStatus.Pending,
                    IsActive = true
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            await _otpService.GenerateAndSendOtpAsync(email, "CaretakerEmailLogin", 5);
            return Ok(new { message = "If the email is valid, an OTP has been sent." });
        }

        [HttpPost("caretaker/verify-email-otp")]
        public async Task<IActionResult> VerifyCaretakerEmailOtp([FromBody] CaretakerEmailOtpVerifyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Otp))
                return BadRequest(new { message = "Email and OTP are required." });

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Role == RoleType.CareTaker && !u.IsDeleted);

            if (user is null)
                return BadRequest(new { message = "Invalid email or OTP." });

            var valid = await _otpService.VerifyOtpAsync(user.UserId, dto.Otp, "CaretakerEmailLogin");
            if (!valid)
                return BadRequest(new { message = "Invalid or expired OTP." });

            if (user.Status != AccountStatus.Approved)
                return Ok(new { status = user.Status.ToString(), message = "Email verified. Awaiting admin approval." });

            var (token, refreshToken) = _authService.GenerateTokensForUser(user);
            return Ok(new { token, refreshToken, role = user.Role.ToString(), message = "Login successful." });
        }


    }
}
