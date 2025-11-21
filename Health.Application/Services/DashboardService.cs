using Health.Application.DTOs.Dashboard;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Health.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardReadRepository _repo;
        private readonly IHttpContextAccessor _context;

        public DashboardService(
            IDashboardReadRepository repo,
            IHttpContextAccessor context)
        {
            _repo = repo;
            _context = context;
        }

        private int CurrentUserId =>
            int.Parse(_context.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public async Task<DashboardSummaryDto> GetPatientDashboardAsync(
            DateTime? date = null,
            int upcomingDays = 7,
            CancellationToken ct = default)
        {
            int userId = CurrentUserId;

            return await _repo.GetPatientDashboardAsync(
                userId,
                date,
                upcomingDays
            );
        }
    }
}
