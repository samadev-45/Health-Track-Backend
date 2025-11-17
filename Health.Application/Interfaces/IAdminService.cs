using System.Collections.Generic;
using System.Threading.Tasks;
using Health.Application.Common;

namespace Health.Application.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<object>> GetPendingUsersAsync();
        Task<ApiResponse<string>> ToggleUserStatusAsync(int userId);
    }
}
