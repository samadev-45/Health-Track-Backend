using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Infrastructure.Repositories.EFCore
{
    public class RefreshTokenRepository
    : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(HealthDbContext context)
            : base(context) { }

        public Task<RefreshToken?> GetByTokenAsync(string token)
            => _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == token && !r.IsDeleted);
    }
}
