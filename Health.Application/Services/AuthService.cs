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

        // Register User
        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            //  Clean up invalid doctor fields
            if (registerDto.Role != RoleType.Doctor)
            {
                // If the user is NOT a doctor, clear these fields
                registerDto.SpecialtyId = 0;
                registerDto.LicenseNumber = null;
            }
            // Validate Email
            if (!IsValidEmail(registerDto.Email))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Invalid email format.");

            //   Validate Phone Numbers
            if (!IsValidPhoneNumber(registerDto.PhoneNumber))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Phone number must contain exactly 10 digits.");

            if (!IsValidPhoneNumber(registerDto.EmergencyContactPhone))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Emergency contact number must contain exactly 10 digits.");


            if (!IsValidEmail(registerDto.Email))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Invalid email format.");

            // Check if email already exists
            var existingUser = await _userRepository.FindAsync(u => u.Email == registerDto.Email);
            if (existingUser.Any())
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Email already registered.");

            //  Role-based validation
             
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

                default:
                    return ApiResponse<RegisterResponseDto>.ErrorResponse("Invalid role type.");
            }


            // Map DTO → Entity
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
                Message = "Registration successful.Awaiting admin approval"
            };

            return ApiResponse<RegisterResponseDto>.SuccessResponse(response, "Registration completed successfully");
        }

        //  Login User
        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            if (!IsValidEmail(loginDto.Email))
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email format.");

            var userList = await _userRepository.FindAsync(u => u.Email == loginDto.Email);
            var user = userList.FirstOrDefault();

            if (user == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("User not found.");

            if (user.Status == AccountStatus.Pending)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Your account is pending approval by admin.");

            if (user.Status == AccountStatus.Rejected)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Your registration was rejected. Please contact admin.");


            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid password.");

            // Generate JWT and Refresh Token
           
            var (accessToken, refreshToken) = _jwtHelper.GenerateTokens(user.UserId, user.Email, user.Role);

            
            _activeRefreshTokens[user.UserId] = refreshToken;
            

            var response = new LoginResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = accessToken,
                RefreshToken = refreshToken
            };


            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
        }

        //  Logout (Revoke Refresh Token)
        public ApiResponse<string> Logout(int userId)
        {
            if (_activeRefreshTokens.ContainsKey(userId))
            {
                _activeRefreshTokens.Remove(userId);
                return ApiResponse<string>.SuccessResponse("Logout successful", "Refresh token revoked");
            }

            return ApiResponse<string>.ErrorResponse("No active session found for this user.");
        }

        //  Generate Secure Refresh Token
        private string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        //  Email Validation
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            //  Disallow leading/trailing special chars like "-", ".", "_"
            if (email.StartsWith("-") || email.StartsWith(".") || email.StartsWith("_") ||
                email.EndsWith("-") || email.EndsWith(".") || email.EndsWith("_"))
                return false;

            // compliant email pattern with stronger local-part validation
            var pattern = @"^(?!.*[-_.]{2})[a-zA-Z0-9]+([._%+-]?[a-zA-Z0-9]+)*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(email, pattern))
                return false;

            //  Domain checks (avoid things like "@.com" or "gmail..com")
            var domain = email.Split('@').LastOrDefault();
            if (string.IsNullOrWhiteSpace(domain) || !domain.Contains('.') || domain.Contains(".."))
                return false;

            //  Prevent invalid symbols like "/", "\", ",", " "
            if (Regex.IsMatch(email, @"[\/\\,\s]"))
                return false;

            return true;
        }
        private bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Allow only 10 digits (no spaces, +, -, etc.)
            var pattern = @"^\d{10}$";
            return Regex.IsMatch(phone, pattern);
        }

        public (string Token, string RefreshToken) GenerateTokensForUser(User user)
        {
            var (accessToken, refreshToken) = _jwtHelper.GenerateTokens(user.UserId, user.Email, user.Role);
            return (accessToken, refreshToken);
        }
        public ApiResponse<LoginResponseDto> RefreshToken(RefreshRequestDto dto)
        {
            // Check if refresh token exists and matches
            if (!_activeRefreshTokens.TryGetValue(dto.UserId, out var storedToken))
                return ApiResponse<LoginResponseDto>.ErrorResponse("No active refresh token found.");

            if (storedToken != dto.RefreshToken)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid refresh token.");

            // Fetch user
            var user = _userRepository.Query().FirstOrDefault(u => u.UserId == dto.UserId);
            if (user == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("User not found.");

            // Generate new tokens
            var (newAccessToken, newRefreshToken) = _jwtHelper.GenerateTokens(user.UserId, user.Email, user.Role);

            // Replace old refresh token
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


    }
}
