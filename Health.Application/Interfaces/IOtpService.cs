using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IOtpService
    {
        Task GenerateAndSendOtpAsync(string email, string purpose, int expiryMinutes = 5);
        Task<bool> VerifyOtpAsync(int userId, string otp, string purpose);
        Task ResetPasswordAsync(int userId, string otp, string newPassword, string confirmPassword);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword);
    }

}
