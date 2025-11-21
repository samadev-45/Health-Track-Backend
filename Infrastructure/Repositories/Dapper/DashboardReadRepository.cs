using Dapper;
using Health.Application.DTOs.Appointment;
using Health.Application.DTOs.Dashboard;
using Health.Application.DTOs.Medication;
using Health.Application.DTOs.HealthMetric;
using Health.Application.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

public class DashboardReadRepository : IDashboardReadRepository
{
    private readonly IConfiguration _config;
    public DashboardReadRepository(IConfiguration config) => _config = config;

    private IDbConnection CreateConnection()
        => new SqlConnection(_config.GetConnectionString("DefaultConnection"));

    public async Task<DashboardSummaryDto> GetPatientDashboardAsync(
        int userId, DateTime? date = null, int upcomingDays = 7)
    {
        using var conn = CreateConnection();

        var result = await conn.QueryMultipleAsync(
            "GetPatientDashboard",
            new
            {
                UserId = userId,
                Date = date,
                UpcomingDays = upcomingDays
            },
            commandType: CommandType.StoredProcedure);

       
        int activeMedCount = await result.ReadSingleAsync<int>();

       
        var todaySchedule = result.Read<MedicationScheduleItemDto>().ToList(); 

        
        var abnormalMetrics = result.Read<AbnormalMetricDto>().ToList();

        
        var latestVitalsRaw = result.Read<dynamic>().ToList();

        var latestVitals = new LatestVitalsDto
        {
            Readings = latestVitalsRaw.Select(x => new VitalReadingDto
            {
                MetricCode = x.MetricCode,
                DisplayName = x.DisplayName,
                Unit = x.Unit,
                Value = x.Value,
                MeasuredAt = x.MeasuredAt
            }).ToList()
        };

       
        var appointments = result.Read<NextAppointmentDto>().ToList();
        var nextAppointment = appointments.FirstOrDefault();


        var notifications = result.Read<NotificationDto>().ToList();

        
        return new DashboardSummaryDto
        {
            NextAppointment = nextAppointment,
            ActiveMedicationCount = activeMedCount,
            TodayAbnormalMetrics = abnormalMetrics,
            LatestVitals = latestVitals,
            RecentNotifications = notifications,
            HealthScore = CalculateHealthScore(abnormalMetrics)
        };
    }

    private int CalculateHealthScore(IEnumerable<AbnormalMetricDto> abnormalMetrics)
    {
        if (!abnormalMetrics.Any()) return 100;

        int penalty = abnormalMetrics.Count() * 10;
        int score = 100 - penalty;
        return Math.Max(score, 40);   
    }
}
