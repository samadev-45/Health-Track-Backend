using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Health.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Health.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly HealthDbContext _context;

        public UserRepository(HealthDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, ct);
        }
        public async Task<IEnumerable<User>> GetDoctorsAsync(CancellationToken ct)
        {
            return await _context.Users
                .Where(u => u.Role == RoleType.Doctor && !u.IsDeleted)
                .ToListAsync(ct);
        }
        public async Task<DoctorProfile?> GetDoctorProfileAsync(int doctorId, CancellationToken ct = default)
        {
            return await _context.DoctorProfiles.FirstOrDefaultAsync(p => p.UserId == doctorId, ct);
        }


    }
}
