using Health.Application.DTOs.Auth;
using Health.Application.Interfaces;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Health.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // ------------------------- REGISTER -------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.RegisterAsync(registerDto);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        // ------------------------- LOGIN -------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(loginDto);

            var msg = (response.Message ?? "").ToLowerInvariant();

            if (!response.Success)
            {
                if (msg.Contains("pending") || msg.Contains("rejected"))
                    return StatusCode(403, response);

                return Unauthorized(response);
            }

            if (response.Data is LoginResponseDto loginData)
            {
                // Set access token cookie
                Response.Cookies.Append("access_token", loginData.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                // Set refresh token cookie
                Response.Cookies.Append("refresh_token", loginData.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                // Remove tokens from response body
                loginData.Token = null!;
                loginData.RefreshToken = null!;
            }

            return Ok(response);
        }

        // ------------------------- FORGOT PASSWORD -------------------------
        [HttpPost("request-email-Otp")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email is required." });

            await _otpService.GenerateAndSendOtpAsync(dto.Email, "ResetPassword");
            return Ok(new { message = "OTP sent successfully." });
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
                await _otpService.ResetPasswordAsync(user.UserId, dto.Otp, dto.NewPassword, dto.ConfirmPassword);
                return Ok(new { message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ------------------------- ADMIN -------------------------
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
            if (user == null)
                return NotFound(new { message = "User not found." });

            var previous = user.Status;

            switch (user.Status)
            {
                case AccountStatus.Pending:
                    user.Status = AccountStatus.Approved; break;
                case AccountStatus.Approved:
                    user.Status = AccountStatus.Rejected; break;
                case AccountStatus.Rejected:
                    user.Status = AccountStatus.Approved; break;
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

        // --------------------- CARETAKER EMAIL LOGIN ---------------------
        [HttpPost("caretaker/request-email-otp")]
        public async Task<IActionResult> RequestCaretakerEmailOtp([FromBody] CaretakerEmailOtpRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest(new { message = "Email is required." });

            var email = dto.Email.Trim().ToLowerInvariant();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Role == RoleType.CareTaker && !u.IsDeleted);

            if (user == null)
            {
                user = new User
                {
                    FullName = string.IsNullOrWhiteSpace(dto.FullName) ? "Caretaker" : dto.FullName.Trim(),
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

            if (user == null)
                return BadRequest(new { message = "Invalid email or OTP." });

            var valid = await _otpService.VerifyOtpAsync(user.UserId, dto.Otp, "CaretakerEmailLogin");
            if (!valid)
                return BadRequest(new { message = "Invalid or expired OTP." });

            if (user.Status != AccountStatus.Approved)
                return Ok(new { status = user.Status.ToString(), message = "Email verified. Awaiting admin approval." });

            var (token, refreshToken) = _authService.GenerateTokensForUser(user);

            // Set cookies — no token in body
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddHours(1)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { role = user.Role.ToString(), message = "Login successful." });
        }

        // ------------------------- REFRESH TOKEN -------------------------
        [HttpPost("refresh")]
        [AllowAnonymous]
        public IActionResult Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized(new { message = "Refresh token missing." });

            var result = _authService.RefreshTokenByValue(refreshToken);
            if (!result.Success)
                return Unauthorized(result);

            if (result.Data is LoginResponseDto data)
            {
                // Rotate tokens in cookies
                Response.Cookies.Append("access_token", data.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                Response.Cookies.Append("refresh_token", data.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                data.Token = null!;
                data.RefreshToken = null!;
            }

            return Ok(result);
        }

        // ------------------------- LOGOUT -------------------------
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (!string.IsNullOrWhiteSpace(refreshToken))
                _authService.RevokeRefreshTokenByValue(refreshToken);

            // Remove cookies
            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            Response.Cookies.Delete("refresh_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
