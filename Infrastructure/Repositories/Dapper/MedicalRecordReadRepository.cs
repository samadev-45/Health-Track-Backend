using Dapper;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.MedicalRecord;
using Health.Application.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Health.Infrastructure.Repositories.Dapper
{
    public class MedicalRecordReadRepository : IMedicalRecordReadRepository
    {
        private readonly IConfiguration _config;
        public MedicalRecordReadRepository(IConfiguration config) => _config = config;

        private IDbConnection CreateConnection() =>
            new SqlConnection(_config.GetConnectionString("DefaultConnection"));

        public async Task<PagedResult<MedicalRecordListDto>> GetForUserAsync(
            int userId, int page, int pageSize, int? recordTypeId,
            DateTime? from, DateTime? to)
        {
            using var conn = CreateConnection();

            var multi = await conn.QueryMultipleAsync(
                "GetMedicalRecordsForUser",
                new
                {
                    UserId = userId,
                    Page = page,
                    PageSize = pageSize,
                    RecordTypeId = recordTypeId,
                    FromDate = from,
                    ToDate = to
                },
                commandType: CommandType.StoredProcedure
            );

            int total = await multi.ReadSingleAsync<int>();
            var items = (await multi.ReadAsync<MedicalRecordListDto>()).ToList();

            return new PagedResult<MedicalRecordListDto>
            {
                Items = items,
                
                TotalCount = total
            };
        }

        public async Task<MedicalRecordDto?> GetByIdAsync(int recordId)
        {
            using var conn = CreateConnection();

            var dto = await conn.QueryFirstOrDefaultAsync<MedicalRecordDto>(
                "GetMedicalRecordById",
                new { RecordId = recordId },
                commandType: CommandType.StoredProcedure
            );

            return dto;
        }
    }
}
