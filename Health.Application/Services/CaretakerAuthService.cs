using Health.Application.Common;
using Health.Application.DTOs.Auth;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Domain.Enums;

public class CaretakerAuthService : ICaretakerAuthService
{
    private readonly IGenericRepository<User> _userRepo;
    private readonly IOtpService _otpService;

    public CaretakerAuthService(
        IGenericRepository<User> userRepo,
        IOtpService otpService)
    {
        _userRepo = userRepo;
        _otpService = otpService;
    }

    public async Task<ApiResponse<string>> RequestOtpAsync(string email, string fullName)
    {
        email = email.Trim().ToLower();

        var users = await _userRepo.FindAsync(u =>
            u.Email == email &&
            u.Role == RoleType.CareTaker &&
            !u.IsDeleted
        );

        var user = users.FirstOrDefault();

        // Auto-create caretaker if not exists
        if (user == null)
        {
            user = new User
            {
                Email = email,
                FullName = string.IsNullOrWhiteSpace(fullName) ? "Caretaker" : fullName.Trim(),
                Role = RoleType.CareTaker,
                Status = AccountStatus.Pending,
                IsActive = true
            };

            await _userRepo.AddAsync(user);
        }

        await _otpService.GenerateAndSendOtpAsync(email, "CaretakerEmailLogin", 5);

        return ApiResponse<string>.SuccessResponse("OTP sent if email is valid.");
    }

    public async Task<ApiResponse<CaretakerEmailOtpVerifyDto>> VerifyOtpAsync(string email, string otp)
    {
        email = email.Trim().ToLower();

        var users = await _userRepo.FindAsync(u =>
            u.Email == email &&
            u.Role == RoleType.CareTaker &&
            !u.IsDeleted
        );

        var user = users.FirstOrDefault();
        if (user == null)
            return ApiResponse<CaretakerEmailOtpVerifyDto>.ErrorResponse("Invalid email.");

        var valid = await _otpService.VerifyOtpAsync(user.UserId, otp, "CaretakerEmailLogin");
        if (!valid)
            return ApiResponse<CaretakerEmailOtpVerifyDto>.ErrorResponse("Invalid or expired OTP.");

        return ApiResponse<CaretakerEmailOtpVerifyDto>.SuccessResponse(new CaretakerEmailOtpVerifyDto
        {
            Email = email,
            Status = user.Status.ToString()
        });
    }
}
