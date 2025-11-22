using Health.Application.DTOs.Admin;

namespace Health.Application.Interfaces.Dapper
{
    public interface IAdminAnalyticsReadRepository
    {
        Task<AdminSummaryDto> GetSummaryAsync();
        Task<IEnumerable<ChartPointDto>> GetAppointmentsChartAsync(int days);
        Task<IEnumerable<AbnormalMetricStatDto>> GetAbnormalMetricStatsAsync();
        Task<IEnumerable<PrescriptionStatDto>> GetPrescriptionStatsAsync(int months);
    }
}
