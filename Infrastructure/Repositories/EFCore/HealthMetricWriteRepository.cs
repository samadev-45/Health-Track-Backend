using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Infrastructure.Data;

namespace Health.Infrastructure.Repositories.EFCore
{
    public class HealthMetricWriteRepository : GenericRepository<HealthMetric>, IHealthMetricWriteRepository
    {
        public HealthMetricWriteRepository(HealthDbContext db) : base(db)
        {
        }
    }
}
