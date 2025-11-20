using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Health.Application.Common;
using Health.Application.Helpers;
using Health.Application.Interfaces;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Health.Application.Interfaces.EFCore;
using Health.Application.DTOs.Auth;

namespace Health.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;

        // Stores: userId => refreshToken
        private static readonly Dictionary<int, string> _activeRefreshTokens = new();

        public AuthService(
            IGenericRepository<User> userRepository,
            JwtHelper jwtHelper,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<User>();
        }

        // ------------------------ REGISTER ------------------------
        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto.Role != RoleType.Doctor)
            {
                registerDto.SpecialtyId = 0;
                registerDto.LicenseNumber = null;
            }

            if (!IsValidEmail(registerDto.Email))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Invalid email format.");

            if (!IsValidPhoneNumber(registerDto.PhoneNumber))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Phone number must contain exactly 10 digits.");

            if (!IsValidPhoneNumber(registerDto.EmergencyContactPhone))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Emergency contact number must contain exactly 10 digits.");

            var existingUser = await _userRepository.FindAsync(u => u.Email == registerDto.Email);
            if (existingUser.Any())
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Email already registered.");

            switch (registerDto.Role)
            {
                case RoleType.Doctor:
                    if (registerDto.SpecialtyId == 0 || string.IsNullOrWhiteSpace(registerDto.LicenseNumber))
                        return ApiResponse<RegisterResponseDto>.ErrorResponse("Doctors must have a valid Specialty ID and License Number.");
                    break;

                case RoleType.Patient:
                case RoleType.CareTaker:
                    if (registerDto.SpecialtyId != 0 || !string.IsNullOrWhiteSpace(registerDto.LicenseNumber))
                        return ApiResponse<RegisterResponseDto>.ErrorResponse("Only doctors can have Specialty ID or License Number.");
                    break;

                case RoleType.Admin:
                    return ApiResponse<RegisterResponseDto>.ErrorResponse("You are not allowed to register as an Admin.");
            }

            var user = _mapper.Map<User>(registerDto);
            user.CreatedOn = DateTime.UtcNow;
            user.Role = registerDto.Role;
            user.Status = AccountStatus.Pending;
            user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

            await _userRepository.AddAsync(user);

            var response = new RegisterResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsEmailVerified = false,
                Message = "Registration successful. Awaiting admin approval"
            };

            return ApiResponse<RegisterResponseDto>.SuccessResponse(response, "Registration completed successfully");
        }

        // ------------------------ LOGIN ------------------------
        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            if (!IsValidEmail(loginDto.Email))
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email format.");

            var userList = await _userRepository.FindAsync(u => u.Email == loginDto.Email);
            var user = userList.FirstOrDefault();

            if (user == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("User not found.");

            if (user.Status == AccountStatus.Pending)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Your account is pending approval by admin.", 403);

            if (user.Status == AccountStatus.Rejected)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Your registration was rejected. Please contact admin.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid password.");

            var (accessToken, refreshToken) = _jwtHelper.GenerateTokens(user.UserId, user.Email, user.Role);

            _activeRefreshTokens[user.UserId] = refreshToken;

            var response = new LoginResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = accessToken,       // controller will move these to cookies
                RefreshToken = refreshToken
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
        }

        // ------------------------ REFRESH (Cookie Only) ------------------------
        public ApiResponse<LoginResponseDto> RefreshTokenByValue(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return ApiResponse<LoginResponseDto>.ErrorResponse("Refresh token missing.");

            var pair = _activeRefreshTokens.FirstOrDefault(kv => kv.Value == refreshToken);

            if (pair.Equals(default(KeyValuePair<int, string>)))
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid refresh token.");

            var userId = pair.Key;

            var user = _userRepository.Query().FirstOrDefault(u => u.UserId == userId && !u.IsDeleted);
            if (user == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("User not found.");

            var (newAccessToken, newRefreshToken) = _jwtHelper.GenerateTokens(user.UserId, user.Email, user.Role);

            _activeRefreshTokens[user.UserId] = newRefreshToken;

            var response = new LoginResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Token refreshed successfully");
        }

        // ------------------------ LOGOUT ------------------------
        public ApiResponse<string> RevokeRefreshTokenByValue(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return ApiResponse<string>.ErrorResponse("Invalid refresh token.");

            var pair = _activeRefreshTokens.FirstOrDefault(kv => kv.Value == refreshToken);

            if (pair.Equals(default(KeyValuePair<int, string>)))
                return ApiResponse<string>.ErrorResponse("Refresh token not found.");

            _activeRefreshTokens.Remove(pair.Key);

            return ApiResponse<string>.SuccessResponse("Refresh token revoked", "Revoked");
        }

        // ------------------------ HELPERS ------------------------
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var pattern = @"^(?!.*[-_.]{2})[a-zA-Z0-9]+([._%+-]?[a-zA-Z0-9]+)*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidPhoneNumber(string phone)
        {
            return Regex.IsMatch(phone ?? "", @"^\d{10}$");
        }

        public (string Token, string RefreshToken) GenerateTokensForUser(User user)
        {
            return _jwtHelper.GenerateTokens(user.UserId, user.Email, user.Role);
        }
    }
}
