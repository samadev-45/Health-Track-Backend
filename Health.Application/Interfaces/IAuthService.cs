using Health.Application.Common;
using Health.Application.DTOs;
using Health.Domain.Entities;


namespace Health.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        (string Token, string RefreshToken) GenerateTokensForUser(User user);

    }
}
