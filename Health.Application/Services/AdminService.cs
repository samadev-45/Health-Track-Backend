using Health.Application.Common;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Microsoft.EntityFrameworkCore;

public class AdminService : IAdminService
{
    private readonly IGenericRepository<User> _userRepo;

    public AdminService(IGenericRepository<User> repo)
    {
        _userRepo = repo;
    }

    

    public async Task<ApiResponse<string>> ToggleUserStatusAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);

        if (user == null)
            return ApiResponse<string>.ErrorResponse("User not found.");

        user.Status = user.Status switch
        {
            AccountStatus.Pending => AccountStatus.Approved,
            AccountStatus.Approved => AccountStatus.Rejected,
            AccountStatus.Rejected => AccountStatus.Approved,
            _ => user.Status
        };

        await _userRepo.UpdateAsync(user);

        return ApiResponse<string>.SuccessResponse($"Status updated to {user.Status}");
    }

    public async Task<IEnumerable<object>> GetPendingUsersAsync()
    {
        return await _userRepo.Query()
            .Where(u => u.Status == AccountStatus.Pending && !u.IsDeleted)
            .Select(u => new {
                u.UserId,
                u.FullName,
                u.Email,
                Status = u.Status.ToString()
            })
            .ToListAsync();
    }

}
