using Health.Application.Interfaces;
using Health.Domain.Entities;
using Health.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Health.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly HealthDbContext _context;
        private readonly IEmailSenderService _emailSender;

        public OtpService(HealthDbContext context, IEmailSenderService emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        private static string HashOtp(string otp)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(otp)));
        }

        // Generate and send OTP (email)
        public async Task GenerateAndSendOtpAsync(string email, string purpose, int expiryMinutes = 5)
        {
            var normEmail = email.Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normEmail);
            if (user == null)
                throw new Exception("User not found.");

            
            var now = DateTime.UtcNow;
            var recent = await _context.OtpVerifications
                .Where(o => o.UserId == user.UserId && o.Purpose == purpose && !o.Used && o.Expiry > now)
                .OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();
            
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            var rec = new OtpVerification
            {
                UserId = user.UserId,
                OtpCodeHash = HashOtp(code),  // store only hash
                Purpose = purpose,
                Expiry = now.AddMinutes(Math.Clamp(expiryMinutes, 1, 10)),
                Used = false,
                Attempts = 0
            };

            _context.OtpVerifications.Add(rec);
            await _context.SaveChangesAsync();

            var body = $"Your OTP code is {code}. It expires in {expiryMinutes} minutes.";
            await _emailSender.SendEmailAsync(normEmail, "Your OTP Code", body);
        }

        // Verify OTP; marks Used on success; enforces attempts
        public async Task<bool> VerifyOtpAsync(int userId, string otp, string purpose)
        {
            var now = DateTime.UtcNow;
            var rec = await _context.OtpVerifications
                .Where(o => o.UserId == userId && o.Purpose == purpose && !o.Used && o.Expiry > now)
                .OrderByDescending(o => o.Id)
                .FirstOrDefaultAsync();

            if (rec is null)
                return false;

            rec.Attempts++;
            if (rec.Attempts > 5)
            {
                rec.Used = true; // lock this OTP record
                await _context.SaveChangesAsync();
                return false;
            }

            var ok = string.Equals(rec.OtpCodeHash, HashOtp(otp), StringComparison.Ordinal);
            if (!ok)
            {
                await _context.SaveChangesAsync();
                return false;
            }

            rec.Used = true; // single-use
            await _context.SaveChangesAsync();
            return true;
        }

        // Forgot password flow continues to call VerifyOtpAsync(userId, otp, "ResetPassword")
        public async Task ResetPasswordAsync(int userId, string otp, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                throw new Exception("Passwords do not match.");

            var valid = await VerifyOtpAsync(userId, otp, "ResetPassword");
            if (!valid)
                throw new Exception("Invalid or expired OTP.");

            var user = await _context.Users.FindAsync(userId) ?? throw new Exception("User not found.");

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                throw new Exception("New password and confirmation do not match.");

            var user = await _context.Users.FindAsync(userId)
                       ?? throw new Exception("User not found.");

            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
                throw new Exception("Current password is incorrect.");

            user.PasswordHash = hasher.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();
        }

    }
}
