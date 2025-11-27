using Dapper;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Infrastructure.Repositories.Dapper
{
    public class AppointmentReadRepository : IAppointmentReadRepository
    {
        private readonly IConfiguration _config;

        public AppointmentReadRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByDoctorAsync(
            int doctorId, int? status,DateTime? date, int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync(ct);

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetAppointmentsByDoctor",
                new { DoctorId = doctorId, Status = status, Page = page, Date= date, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var appointments = await multi.ReadAsync<Appointment>();

            return (appointments, totalCount);
        }

        public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByPatientAsync(
            int patientId, int? status, int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync(ct);

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetAppointmentsByPatient",
                new { PatientId = patientId, Status = status, Page = page, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var appointments = await multi.ReadAsync<Appointment>();

            return (appointments, totalCount);
        }
        public async Task<IEnumerable<AppointmentHistory>> GetAppointmentHistoryAsync(
    int appointmentId,
    CancellationToken ct = default)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync(ct);

            var result = await connection.QueryAsync<AppointmentHistory, string, AppointmentHistory>(
                "sp_GetAppointmentHistory",
                (history, changedByName) =>
                {
                    history.ChangedByUser = new Health.Domain.Entities.User
                    {
                        FullName = changedByName
                    };
                    return history;
                },
                new { AppointmentId = appointmentId },
                commandType: CommandType.StoredProcedure,
                splitOn: "ChangedByName"
            );

            return result.ToList();
        }

        public async Task<AppointmentHistory?> GetLastCancelledAppointmentAsync(int patientId, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync(ct);

            return await connection.QueryFirstOrDefaultAsync<AppointmentHistory>(
                "sp_GetLastCancelledAppointment",
                new { PatientId = patientId },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorAndDateAsync(
    int doctorId, DateTime date, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync(ct);

            var sql = @"
        SELECT *
        FROM Appointments
        WHERE DoctorId = @DoctorId
          AND AppointmentDate = @Date
          AND IsDeleted = 0";

            return await connection.QueryAsync<Appointment>(sql, new
            {
                DoctorId = doctorId,
                Date = date.Date
            });
        }




    }
}
