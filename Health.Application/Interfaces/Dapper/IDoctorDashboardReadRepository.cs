using Health.Application.DTOs.Dashboard;

namespace Health.Application.Interfaces.Dapper
{
    public interface IDoctorDashboardReadRepository
    {
        Task<DoctorDashboardDto> GetDoctorDashboardAsync(int doctorId, DateTime? date = null, int upcomingDays = 7, int recentCount = 10);
    }
}
