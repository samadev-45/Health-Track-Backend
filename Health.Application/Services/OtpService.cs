using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Microsoft.AspNetCore.Identity;

public class OtpService : IOtpService
{
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<OtpVerification> _otpRepo;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IEmailSenderService _email;

    public OtpService(
        IGenericRepository<User> userRepo,
        IGenericRepository<OtpVerification> otpRepo,
        IEmailSenderService email)
    {
        _userRepo = userRepo;
        _otpRepo = otpRepo;
        _email = email;
        _hasher = new PasswordHasher<User>();
    }

    public async Task GenerateAndSendOtpAsync(string email, string purpose, int expiryMinutes = 5)
    {
        var user = (await _userRepo.FindAsync(u => u.Email == email)).FirstOrDefault();

        if (user == null) return;

        var otp = new Random().Next(100000, 999999).ToString();

        var record = new OtpVerification
        {
            UserId = user.UserId,
            OtpCode = int.Parse(otp),  // FIX 1: Convert string → int
            Purpose = purpose,
            Expiry = DateTime.UtcNow.AddMinutes(expiryMinutes) // FIX 2: Correct property name
        };

        await _otpRepo.AddAsync(record);
        await _email.SendEmailAsync(email, "Your OTP Code", $"Your OTP is {otp}");
    }

    public async Task<bool> VerifyOtpAsync(int userId, string otp, string purpose)
    {
        var records = await _otpRepo.FindAsync(o =>
            o.UserId == userId && o.Purpose == purpose
        );

        var record = records
            .Where(r => r.Expiry > DateTime.UtcNow)   // FIX 3: Expiry not ExpiresAt
            .OrderByDescending(r => r.Expiry)
            .FirstOrDefault();

        if (record == null)
            return false;

        return record.OtpCode == int.Parse(otp); // FIX 4: compare int to int
    }

    public async Task ResetPasswordAsync(int userId, string otp, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
            throw new Exception("Passwords do not match.");

        var valid = await VerifyOtpAsync(userId, otp, "ResetPassword");
        if (!valid)
            throw new Exception("Invalid or expired OTP.");

        var user = await _userRepo.GetByIdAsync(userId);
        user.PasswordHash = _hasher.HashPassword(user, newPassword);

        await _userRepo.UpdateAsync(user);
    }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword)
    {
        var user = await _userRepo.GetByIdAsync(userId);

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Current password incorrect.");

        if (newPassword != confirmPassword)
            throw new Exception("Passwords do not match.");

        user.PasswordHash = _hasher.HashPassword(user, newPassword);

        await _userRepo.UpdateAsync(user);
    }
}
