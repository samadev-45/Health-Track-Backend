using Health.Application.DTOs.Admin;

namespace Health.Application.Interfaces
{
    public interface IAdminAnalyticsService
    {
        Task<AdminSummaryDto> GetSummaryAsync();
        Task<IEnumerable<ChartPointDto>> GetAppointmentsChartAsync(int days);
        Task<IEnumerable<AbnormalMetricStatDto>> GetAbnormalMetricsAsync();
        Task<IEnumerable<PrescriptionStatDto>> GetPrescriptionStatsAsync(int months);
    }
}
