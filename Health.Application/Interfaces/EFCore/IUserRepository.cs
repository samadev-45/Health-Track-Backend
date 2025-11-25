using Health.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.EFCore
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int userId, CancellationToken ct = default);
        Task<IEnumerable<User>> GetDoctorsAsync(CancellationToken ct);
        Task<DoctorProfile?> GetDoctorProfileAsync(int doctorId, CancellationToken ct = default);


    }
}
