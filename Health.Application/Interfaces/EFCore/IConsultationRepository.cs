using Health.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.EFCore
{
    public interface IConsultationRepository : IGenericRepository<Consultation>
    {
        Task<IEnumerable<Consultation>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    }
}
