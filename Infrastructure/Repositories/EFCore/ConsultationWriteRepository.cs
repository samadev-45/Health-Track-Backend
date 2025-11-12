using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Infrastructure.Repositories.EFCore
{
    public class ConsultationWriteRepository : GenericRepository<Consultation>, IConsultationWriteRepository
    {
        public ConsultationWriteRepository(HealthDbContext context) : base(context) { }

        public async Task<IEnumerable<Consultation>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(c => c.Appointment)
                .Where(c => c.PatientId == userId)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync(ct);
        }
    }
}
