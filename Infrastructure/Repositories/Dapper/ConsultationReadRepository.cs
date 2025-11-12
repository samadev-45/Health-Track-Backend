using Dapper;
using Health.Application.DTOs;
using Health.Application.DTOs.Common;
using Health.Application.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Health.Infrastructure.Repositories.Dapper
{
    public class ConsultationReadRepository : IConsultationReadRepository
    {
        private readonly string _connectionString;

        public ConsultationReadRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByDoctorAsync(
    int doctorId,
    int? status = null,
    DateTime? fromDate = null,
    DateTime? toDate = null,
    int page = 1,
    int pageSize = 20)
        {
            using var conn = new SqlConnection(_connectionString);

            using var multi = await conn.QueryMultipleAsync(
                "sp_GetConsultationsByDoctor",
                new
                {
                    DoctorId = doctorId,
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Page = page,
                    PageSize = pageSize
                },
                commandType: CommandType.StoredProcedure);

            int totalCount = await multi.ReadFirstAsync<int>();
            var items = await multi.ReadAsync<ConsultationListDto>();

            return new PagedResult<ConsultationListDto>
            {
                TotalCount = totalCount,
                Items = items.ToList()
            };
        }


        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByPatientAsync(int patientId, int? status = null, DateTime? fromDate = null, DateTime? toDate = null,
    int page = 1,
    int pageSize = 10)

        {
            using var conn = new SqlConnection(_connectionString);

            using var multi = await conn.QueryMultipleAsync(
                "sp_GetConsultationsByPatient",
                new
                {
                    PatientId = patientId,
                    Status = status,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Page = page,
                    PageSize = pageSize
                },
                commandType: CommandType.StoredProcedure);

            var totalCount = await multi.ReadFirstAsync<int>();
            var items = await multi.ReadAsync<ConsultationListDto>();

            return new PagedResult<ConsultationListDto>
            {
                TotalCount = totalCount,
                Items = items.ToList()
            };
        }


        public async Task<ConsultationDetailsDto?> GetConsultationDetailsAsync(int consultationId)
        {
            using var conn = new SqlConnection(_connectionString);

            using var multi = await conn.QueryMultipleAsync(
                "sp_GetConsultationDetails",
                new { ConsultationId = consultationId },
                commandType: CommandType.StoredProcedure);

            var consultation = await multi.ReadFirstOrDefaultAsync<ConsultationDetailsDto>();
            if (consultation == null)
                return null;

            var items = await multi.ReadAsync<PrescriptionItemDto>();
            var files = await multi.ReadAsync<FileDto>();

            consultation.PrescriptionItems = items.ToList();
            consultation.Attachments = files.ToList();

            return consultation;
        }


        public async Task<PagedResult<FileDto>> GetConsultationFilesAsync(
    int consultationId,
    int page = 1,
    int pageSize = 20)
        {
            using var conn = new SqlConnection(_connectionString);

            using var multi = await conn.QueryMultipleAsync(
                "sp_GetConsultationFiles",
                new { ConsultationId = consultationId, Page = page, PageSize = pageSize },
                commandType: CommandType.StoredProcedure);

            var totalCount = await multi.ReadFirstAsync<int>();
            var files = await multi.ReadAsync<FileDto>();

            return new PagedResult<FileDto>
            {
                TotalCount = totalCount,
                Items = files.ToList()
            };
        }

    }
}
