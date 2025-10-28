using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using Health.Application.Common;
using Health.Application.DTOs;
using Health.Application.Helpers;
using Health.Application.Interfaces;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Health.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;

        // Store refresh tokens in-memory (no DB)
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

        // 🔹 Register User
        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (!IsValidEmail(registerDto.Email))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Invalid email format.");

            // Check if email already exists
            var existingUser = await _userRepository.FindAsync(u => u.Email == registerDto.Email);
            if (existingUser.Any())
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Email already registered.");

            // 🧠 Role-based validation
            if (registerDto.Role == RoleType.Doctor)
            {
                if (registerDto.SpecialtyId == 0 || string.IsNullOrWhiteSpace(registerDto.LicenseNumber))
                    return ApiResponse<RegisterResponseDto>.ErrorResponse("Doctors must have a valid Specialty ID and License Number.");
            }
            else
            {
                if (registerDto.SpecialtyId != 0 || !string.IsNullOrWhiteSpace(registerDto.LicenseNumber))
                    return ApiResponse<RegisterResponseDto>.ErrorResponse("Only doctors can have Specialty ID or License Number.");
            }

            // Map DTO → Entity
            var user = _mapper.Map<User>(registerDto);
            user.CreatedOn = DateTime.UtcNow;
            user.Role = registerDto.Role == 0 ? RoleType.Patient : registerDto.Role;
            user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

            await _userRepository.AddAsync(user);

            var response = new RegisterResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                IsEmailVerified = false,
                Message = "Registration successful"
            };

            return ApiResponse<RegisterResponseDto>.SuccessResponse(response, "Registration completed successfully");
        }

        // 🔹 Login User
        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            if (!IsValidEmail(loginDto.Email))
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid email format.");

            var userList = await _userRepository.FindAsync(u => u.Email == loginDto.Email);
            var user = userList.FirstOrDefault();

            if (user == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("User not found.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
            if (result == PasswordVerificationResult.Failed)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid password.");

            // Generate JWT and Refresh Token
            var token = _jwtHelper.GenerateToken(user.UserId, user.Email, user.Role);
            var refreshToken = GenerateRefreshToken();
            _activeRefreshTokens[user.UserId] = refreshToken; // in-memory

            var response = new LoginResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token,
                RefreshToken = refreshToken
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
        }

        // 🔹 Logout (Revoke Refresh Token)
        public ApiResponse<string> Logout(int userId)
        {
            if (_activeRefreshTokens.ContainsKey(userId))
            {
                _activeRefreshTokens.Remove(userId);
                return ApiResponse<string>.SuccessResponse("Logout successful", "Refresh token revoked");
            }

            return ApiResponse<string>.ErrorResponse("No active session found for this user.");
        }

        // 🔹 Generate Secure Refresh Token
        private string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        //  Email Validation
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern)) return false;

            var domain = email.Split('@').LastOrDefault();
            if (domain == null || !domain.Contains('.')) return false;
            if (domain.StartsWith('.') || domain.EndsWith('.')) return false;

            return true;
        }
    }
}
