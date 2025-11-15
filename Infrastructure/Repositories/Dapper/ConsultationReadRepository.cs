using Dapper;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.Consultation;
using Health.Application.DTOs.File;
using Health.Application.DTOs.Prescription;
using Health.Application.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
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

        // ------------------------------------------------------------
        // DOCTOR LIST
        // ------------------------------------------------------------
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

            var totalCount = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<ConsultationListDto>()).ToList();

            return new PagedResult<ConsultationListDto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }

        // ------------------------------------------------------------
        // PATIENT LIST
        // ------------------------------------------------------------
        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByPatientAsync(
            int patientId,
            int? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 20)
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

            var totalCount = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<ConsultationListDto>()).ToList();

            return new PagedResult<ConsultationListDto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }

        // ------------------------------------------------------------
        // CONSULTATION DETAILS
        // ------------------------------------------------------------
        public async Task<ConsultationDetailsDto?> GetConsultationDetailsAsync(int consultationId)
        {
            using var conn = new SqlConnection(_connectionString);

            using var multi = await conn.QueryMultipleAsync(
                "sp_GetConsultationDetails",
                new { ConsultationId = consultationId },
                commandType: CommandType.StoredProcedure);

            var details = await multi.ReadFirstOrDefaultAsync<ConsultationDetailsDto>();

            if (details == null)
                return null;

            // Deserialize JSON for HealthValues
            if (!string.IsNullOrWhiteSpace(details.HealthValuesJson))
            {
                try
                {
                    details.HealthValues = JsonSerializer.Deserialize<Dictionary<string, decimal>>(
                        details.HealthValuesJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch
                {
                    details.HealthValues = new Dictionary<string, decimal>();
                }
            }

            // Prescription items
            details.PrescriptionItems = (await multi.ReadAsync<PrescriptionItemDto>()).ToList();

            // Attachments
            details.Attachments = (await multi.ReadAsync<FileDto>()).ToList();

            return details;
        }

        // ------------------------------------------------------------
        // FILE LIST
        // ------------------------------------------------------------
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

            var totalCount = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<FileDto>()).ToList();

            return new PagedResult<FileDto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }
    }
}
