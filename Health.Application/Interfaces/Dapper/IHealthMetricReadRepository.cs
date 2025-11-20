using Health.Application.DTOs.Common;
using Health.Application.DTOs.HealthMetric;

namespace Health.Application.Interfaces.Dapper
{
    public interface IHealthMetricReadRepository
    {
        
        Task<PagedResult<HealthMetricListDto>> GetForUserAsync(
            int userId,
            int page,
            int pageSize,
            int? metricTypeId = null,
            DateTime? from = null,
            DateTime? to = null);

        
        Task<HealthMetricDetailsDto?> GetByIdAsync(int healthMetricId);

        
        Task<IEnumerable<HealthMetricListDto>> GetTodayAbnormalAsync(
            int userId,
            DateTime todayDate);

       
        Task<IEnumerable<TrendPoint>> GetTrendHistoryAsync(
            int userId,
            int metricTypeId,
            int days);
    }
}
