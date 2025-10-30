using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IOtpService
    {
        Task GenerateAndSendOtpAsync(string email, string purpose);
        Task<bool> VerifyOtpAsync(int userId, int otpCode, string purpose);
        Task ResetPasswordAsync(int userId, int otpCode, string newPassword, string confirmPassword);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword);
    }
}
