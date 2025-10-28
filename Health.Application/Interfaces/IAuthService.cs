using Health.Application.Common;
using Health.Application.DTOs;


namespace Health.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto loginDto);
    }
}
