

using AutoMapper;
using Health.Application.Common;
using Health.Application.DTOs.Auth;
using Health.Application.Helpers;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Health.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepo;
        private readonly IDoctorProfileRepository _doctorProfileRepo;

        private readonly JwtHelper _jwtHelper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;

        public AuthService(
            IGenericRepository<User> userRepo,
            IGenericRepository<RefreshToken> refreshTokenRepo,
            IDoctorProfileRepository doctorProfileRepo,
            JwtHelper jwtHelper,
            IConfiguration config,
            IMapper mapper)
        {
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
            _doctorProfileRepo = doctorProfileRepo;
            _jwtHelper = jwtHelper;
            _mapper = mapper;
            _passwordHasher = new PasswordHasher<User>();
        }

        // ========================= REGISTER =========================
        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto dto)
        {
            if (!IsValidEmail(dto.Email))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Invalid email format.");

            if (!IsValidPhoneNumber(dto.PhoneNumber))
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Phone must be 10 digits.");

            var existing = await _userRepo.FindAsync(u => u.Email == dto.Email);
            if (existing.Any())
                return ApiResponse<RegisterResponseDto>.ErrorResponse("Email already registered.");

            var user = _mapper.Map<User>(dto);

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            user.Status = AccountStatus.Pending;
            user.CreatedOn = DateTime.UtcNow;

            await _userRepo.AddAsync(user);

            if (user.Role == RoleType.Doctor)
            {
                await _doctorProfileRepo.AddAsync(new DoctorProfile
                {
                    UserId = user.UserId
                });
            }

            var response = new RegisterResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Message = "Registration successful."
            };

            return ApiResponse<RegisterResponseDto>.SuccessResponse(response, "OK");
        }

        // ========================= LOGIN =========================
        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            var users = await _userRepo.FindAsync(u => u.Email == dto.Email);
            var user = users.FirstOrDefault();

            if (user == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("User not found.");

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verify == PasswordVerificationResult.Failed)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid password.");

            // generate tokens
            var (accessToken, refreshToken) = _jwtHelper.GenerateTokens(user.UserId, user.Email, user.Role);

            // save refresh token into DB
            var refreshEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepo.AddAsync(refreshEntity);

            var result = new LoginResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = accessToken,
                RefreshToken = refreshToken
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(result, "Login successful");
        }

        // ========================= REFRESH TOKEN =========================
        public ApiResponse<LoginResponseDto> RefreshTokenByValue(string refreshToken)
        {
            var token = _refreshTokenRepo
                .Query()
                .FirstOrDefault(t => t.Token == refreshToken && !t.IsDeleted && !t.IsRevoked);

            if (token == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid refresh token.");

            if (token.ExpiresAt < DateTime.UtcNow)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Refresh token expired.");

            var user = _userRepo.Query().FirstOrDefault(u => u.UserId == token.UserId);
            if (user == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("User not found.");

            // revoke old token
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            _refreshTokenRepo.UpdateAsync(token);

            // generate new tokens
            var (newAccess, newRefresh) = _jwtHelper.GenerateTokens(user.UserId, user.Email, user.Role);

            // save new refresh token
            var newEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = newRefresh,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            _refreshTokenRepo.AddAsync(newEntity);

            var response = new LoginResponseDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = newAccess,
                RefreshToken = newRefresh
            };

            return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Token refreshed");
        }

        // ========================= LOGOUT =========================
        public ApiResponse<string> RevokeRefreshTokenByValue(string refreshToken)
        {
            var token = _refreshTokenRepo
                .Query()
                .FirstOrDefault(t => t.Token == refreshToken && !t.IsDeleted);

            if (token == null)
                return ApiResponse<string>.ErrorResponse("Token not found.");

            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            _refreshTokenRepo.UpdateAsync(token);

            return ApiResponse<string>.SuccessResponse("Logged out", "OK");
        }

        // ========================= HELPERS =========================
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
