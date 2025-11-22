using Dapper;
using Health.Application.DTOs.Admin;
using Health.Application.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


    public class AdminAnalyticsReadRepository : IAdminAnalyticsReadRepository
    {
        private readonly IConfiguration _config;

        public AdminAnalyticsReadRepository(IConfiguration config)
        {
            _config = config;
        }

        private IDbConnection CreateConnection() =>
            new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        public async Task<AdminSummaryDto> GetSummaryAsync()
        {
            using var conn = CreateConnection();

            var multi = await conn.QueryMultipleAsync(
                "GetAdminAnalyticsSummary",
                commandType: CommandType.StoredProcedure
            );

            return new AdminSummaryDto
            {
                TotalUsers = multi.ReadFirst<int>(),
                TotalDoctors = multi.ReadFirst<int>(),
                TotalPatients = multi.ReadFirst<int>(),
                TodayAppointments = multi.ReadFirst<int>(),
                WeekAppointments = multi.ReadFirst<int>(),
                ActiveMedications = multi.ReadFirst<int>(),
                TodayAbnormalMetrics = multi.ReadFirst<int>(),
                TotalPrescriptions = multi.ReadFirst<int>()
            };
        }

        public async Task<IEnumerable<ChartPointDto>> GetAppointmentsChartAsync(int days)
        {
            using var conn = CreateConnection();

            var data = (await conn.QueryAsync<ChartPointDto>(
                "GetAdminAppointmentsChart",
                new { Days = days },
                commandType: CommandType.StoredProcedure
            ));

            return data.ToList();
        }

        public async Task<IEnumerable<AbnormalMetricStatDto>> GetAbnormalMetricStatsAsync()
        {
            using var conn = CreateConnection();

            var data = (await conn.QueryAsync<AbnormalMetricStatDto>(
                "GetAdminMetricAbnormalStats",
                commandType: CommandType.StoredProcedure
            ));

            return data.ToList();
        }

        public async Task<IEnumerable<PrescriptionStatDto>> GetPrescriptionStatsAsync(int months)
        {
            using var conn = CreateConnection();

            var data = (await conn.QueryAsync<PrescriptionStatDto>(
                "GetAdminPrescriptionStats",
                new { Months = months },
                commandType: CommandType.StoredProcedure
            ));

            return data.ToList();
        }
    


}
