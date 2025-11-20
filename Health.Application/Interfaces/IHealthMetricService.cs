using Health.Application.DTOs.Common;
using Health.Application.DTOs.HealthMetric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IHealthMetricService
    {
        Task<HealthMetricDetailsDto> CreateAsync(CreateHealthMetricDto dto, CancellationToken ct = default);
         Task<HealthMetricDetailsDto> UpdateAsync(int id, HealthMetricUpdateDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<HealthMetricDetailsDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<HealthMetricListDto>> GetTodayAbnormalAsync(CancellationToken ct = default);
        Task<MetricTrendDto> GetTrendAsync(int metricTypeId, int days, CancellationToken ct = default);
         
        Task<PagedResult<HealthMetricListDto>> GetForUserAsync(
        int page, int pageSize, int? metricTypeId, DateTime? from, DateTime? to,
        CancellationToken ct = default);
        
    }
}
