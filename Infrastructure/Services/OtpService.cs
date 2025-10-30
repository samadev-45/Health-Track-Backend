using Health.Application.Interfaces;
using Health.Domain.Entities;
using Health.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task GenerateAndSendOtpAsync(string email, string purpose)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new Exception("User not found.");

            // ✅ Generate numeric OTP (6 digits)
            var otpCode = new Random().Next(100000, 999999);
            var expiryTime = DateTime.UtcNow.AddMinutes(10);

            var otp = new OtpVerification
            {
                UserId = user.UserId,
                OtpCode = otpCode,  // now int
                Purpose = purpose,
                Expiry = expiryTime,
                Used = false
            };

            await _context.OtpVerifications.AddAsync(otp);
            await _context.SaveChangesAsync();

            string emailBody = $"Your OTP code is {otpCode}. It expires at {expiryTime.ToLocalTime():f}.";
            await _emailSender.SendEmailAsync(email, "Your OTP Code", emailBody);
        }

        public async Task<bool> VerifyOtpAsync(int userId, int otpCode, string purpose)
        {
            var otp = await _context.OtpVerifications
                .Where(o => o.UserId == userId && o.OtpCode == otpCode && o.Purpose == purpose && !o.Used)
                .OrderByDescending(o => o.Expiry)
                .FirstOrDefaultAsync();

            if (otp == null || otp.Expiry < DateTime.UtcNow)
                return false;

            otp.Used = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ResetPasswordAsync(int userId, int otpCode, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                throw new Exception("Passwords do not match.");

            var isOtpValid = await VerifyOtpAsync(userId, otpCode, "ResetPassword");
            if (!isOtpValid)
                throw new Exception("Invalid or expired OTP.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                throw new Exception("New password and confirmation do not match.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Current password is incorrect.");

            user.PasswordHash = hasher.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();
        }
    }
}
