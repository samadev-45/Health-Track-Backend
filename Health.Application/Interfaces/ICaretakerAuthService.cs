using System.Threading.Tasks;
using Health.Application.Common;
using Health.Application.DTOs.Auth;

namespace Health.Application.Interfaces
{
    public interface ICaretakerAuthService
    {
        Task<ApiResponse<string>> RequestOtpAsync(string email, string fullName);
        Task<ApiResponse<CaretakerEmailOtpVerifyDto>> VerifyOtpAsync(string email, string otp);
    }
}
