using Health.Application.DTOs.Dashboard;

namespace Health.Application.Interfaces
{
    public interface IDoctorDashboardService
    {
        Task<DoctorDashboardDto> GetDoctorDashboardAsync(DateTime? date = null, int upcomingDays = 7, int recentCount = 10, CancellationToken ct = default);
    }
}
