using Health.Application.DTOs.Admin;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;

public class AdminAnalyticsService : IAdminAnalyticsService
{
    private readonly IAdminAnalyticsReadRepository _repo;

    public AdminAnalyticsService(IAdminAnalyticsReadRepository repo)
    {
        _repo = repo;
    }

    public Task<AdminSummaryDto> GetSummaryAsync() =>
        _repo.GetSummaryAsync();

    public Task<IEnumerable<ChartPointDto>> GetAppointmentsChartAsync(int days) =>
        _repo.GetAppointmentsChartAsync(days);

    public Task<IEnumerable<AbnormalMetricStatDto>> GetAbnormalMetricsAsync() =>
        _repo.GetAbnormalMetricStatsAsync();

    public Task<IEnumerable<PrescriptionStatDto>> GetPrescriptionStatsAsync(int months) =>
        _repo.GetPrescriptionStatsAsync(months);
}
