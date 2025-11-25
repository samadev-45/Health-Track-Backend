using Health.Application.Interfaces;
using Health.Domain.Entities;
using Health.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Infrastructure.Repositories.EFCore
{
    public class DoctorProfileRepository
        : GenericRepository<DoctorProfile>, IDoctorProfileRepository
    {
        public DoctorProfileRepository(HealthDbContext context)
            : base(context)
        {
        }

        public async Task<DoctorProfile?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId, ct);
        }
    }
}
