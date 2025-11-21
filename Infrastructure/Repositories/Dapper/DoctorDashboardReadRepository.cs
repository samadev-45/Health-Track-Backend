using Dapper;
using Health.Application.DTOs.Dashboard;
using Health.Application.DTOs.HealthMetric;
using Health.Application.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

public class DoctorDashboardReadRepository : IDoctorDashboardReadRepository
{
    private readonly IConfiguration _config;
    public DoctorDashboardReadRepository(IConfiguration config) => _config = config;

    private IDbConnection CreateConnection() =>
        new SqlConnection(_config.GetConnectionString("DefaultConnection"));

    public async Task<DoctorDashboardDto> GetDoctorDashboardAsync(int doctorId, DateTime? date = null, int upcomingDays = 7, int recentCount = 10)
    {
        using var conn = CreateConnection();

        var multi = await conn.QueryMultipleAsync(
            "GetDoctorDashboard",
            new
            {
                DoctorId = doctorId,
                Date = date,
                UpcomingDays = upcomingDays,
                RecentCount = recentCount
            },
            commandType: CommandType.StoredProcedure);

        var todayAppointments = (await multi.ReadAsync<DoctorAppointmentItemDto>()).ToList();
        var upcoming = (await multi.ReadAsync<DoctorAppointmentItemDto>()).ToList();
        var consultations = (await multi.ReadAsync<DoctorConsultationItemDto>()).ToList();
        var patients = (await multi.ReadAsync<DoctorPatientItemDto>()).ToList();
        var abnormalMetrics = (await multi.ReadAsync<AbnormalMetricDto>()).ToList();
        var notifications = (await multi.ReadAsync<NotificationDto>()).ToList();

        return new DoctorDashboardDto
        {
            TodayAppointments = todayAppointments,
            UpcomingAppointments = upcoming,
            RecentConsultations = consultations,
            Patients = patients,
            AbnormalMetrics = abnormalMetrics,
            Notifications = notifications
        };
    }
}
