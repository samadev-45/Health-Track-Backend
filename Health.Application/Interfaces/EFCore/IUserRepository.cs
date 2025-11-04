using Health.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.EFCore
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId, CancellationToken ct = default);
    }
}
