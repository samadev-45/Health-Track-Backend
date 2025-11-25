using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IDoctorProfileRepository : IGenericRepository<DoctorProfile>
    {
        Task<DoctorProfile?> GetByUserIdAsync(int userId, CancellationToken ct = default);
        Task AddAsync(DoctorProfile profile);
        Task UpdateAsync(DoctorProfile profile);
    }
}
