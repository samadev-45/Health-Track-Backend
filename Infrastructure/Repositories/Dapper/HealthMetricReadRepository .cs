using Dapper;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.HealthMetric;
using Health.Application.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Health.Infrastructure.Repositories.Dapper
{
    public class HealthMetricReadRepository : IHealthMetricReadRepository
    {
        private readonly IConfiguration _config;

        public HealthMetricReadRepository(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection CreateConnection()
            => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        // ---------------------- LIST WITH PAGING ----------------------
        public async Task<PagedResult<HealthMetricListDto>> GetForUserAsync(
            int userId,
            int page,
            int pageSize,
            int? metricTypeId = null,
            DateTime? from = null,
            DateTime? to = null)
        {
            using var conn = CreateConnection();

            var multi = await conn.QueryMultipleAsync(
                "GetHealthMetricsForUser",
                new
                {
                    UserId = userId,
                    Page = page,
                    PageSize = pageSize,
                    MetricTypeId = metricTypeId,
                    From = from,
                    To = to
                },
                commandType: CommandType.StoredProcedure);

            int total = multi.ReadSingle<int>();
            var items = multi.Read<HealthMetricListDto>().ToList();

            return new PagedResult<HealthMetricListDto>
            {
                Items = items,
                TotalCount = total
            };
        }

        // ---------------------- DETAILS ----------------------
        public async Task<HealthMetricDetailsDto?> GetByIdAsync(int metricId)
        {
            using var conn = CreateConnection();

            return await conn.QueryFirstOrDefaultAsync<HealthMetricDetailsDto>(
                "GetHealthMetricById",
                new { HealthMetricId = metricId },
                commandType: CommandType.StoredProcedure);
        }

        // ---------------------- TODAY ABNORMAL ----------------------
        public async Task<IEnumerable<HealthMetricListDto>> GetTodayAbnormalAsync(int userId, DateTime date)
        {
            using var conn = CreateConnection();

            return await conn.QueryAsync<HealthMetricListDto>(
                "GetTodayAbnormalMetricsForUser",
                new { UserId = userId, Date = date },
                commandType: CommandType.StoredProcedure);
        }

        // ---------------------- LATEST SUMMARY ----------------------
        public async Task<IEnumerable<HealthMetricListDto>> GetLatestForUserAsync(int userId)
        {
            using var conn = CreateConnection();

            return await conn.QueryAsync<HealthMetricListDto>(
                "GetLatestHealthMetricsForUser",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        // ---------------------- TRENDS ----------------------
        public async Task<IEnumerable<TrendPoint>> GetTrendHistoryAsync(int userId, int metricTypeId, int days)
        {
            using var conn = CreateConnection();

            var data = await conn.QueryAsync<dynamic>(
                "GetHealthMetricHistoryForUser",
                new
                {
                    UserId = userId,
                    MetricTypeId = metricTypeId,
                    Days = days
                },
                commandType: CommandType.StoredProcedure
            );

            return data.Select(x => new TrendPoint
            {
                MeasuredAt = x.MeasuredAt,
                Value = x.Value,
                IsAbnormal = x.AbnormalStatus == "High" || x.AbnormalStatus == "Low"
            });
        }

    }
}
