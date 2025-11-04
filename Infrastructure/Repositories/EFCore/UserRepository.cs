using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
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
    }
}
