using Health.Application.DTOs.Dashboard;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class DoctorDashboardService : IDoctorDashboardService
{
    private readonly IDoctorDashboardReadRepository _repo;
    private readonly IHttpContextAccessor _context;

    public DoctorDashboardService(IDoctorDashboardReadRepository repo, IHttpContextAccessor context)
    {
        _repo = repo;
        _context = context;
    }

    private int CurrentUserId => int.Parse(_context.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public async Task<DoctorDashboardDto> GetDoctorDashboardAsync(DateTime? date = null, int upcomingDays = 7, int recentCount = 10, CancellationToken ct = default)
    {
       
        int doctorId = CurrentUserId;
        return await _repo.GetDoctorDashboardAsync(doctorId, date, upcomingDays, recentCount);
    }
}
