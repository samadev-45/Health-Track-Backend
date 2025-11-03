using Dapper;
using Health.Application.Interfaces;
using Health.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Health.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IConfiguration _configuration;

        public AppointmentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByDoctorAsync(int doctorId, int? status, int page, int pageSize)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetAppointmentsByDoctor",
                new { DoctorId = doctorId, Status = status, Page = page, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var appointments = await multi.ReadAsync<Appointment>();

            return (appointments, totalCount);
        }

        public async Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByPatientAsync(int patientId, int? status, int page, int pageSize)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();

            using var multi = await connection.QueryMultipleAsync(
                "sp_GetAppointmentsByPatient",
                new { PatientId = patientId, Status = status, Page = page, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            var totalCount = await multi.ReadFirstAsync<int>();
            var appointments = await multi.ReadAsync<Appointment>();

            return (appointments, totalCount);
        }
    }
}
