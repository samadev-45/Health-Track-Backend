using Dapper;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.Medication;
using Health.Application.Interfaces.Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

public class MedicationReadRepository : IMedicationReadRepository
{
    private readonly IConfiguration _config;
    public MedicationReadRepository(IConfiguration config) => _config = config;

    private IDbConnection CreateConnection() =>
        new SqlConnection(_config.GetConnectionString("DefaultConnection"));

    public async Task<PagedResult<MedicationListDto>> GetForUserAsync(int userId, int page, int pageSize)
    {
        using var conn = CreateConnection();

        var multi = await conn.QueryMultipleAsync("GetMedicationsForUser",
            new { UserId = userId, Page = page, PageSize = pageSize }, commandType: CommandType.StoredProcedure);

        int total = await multi.ReadSingleAsync<int>();
        var items = (await multi.ReadAsync<MedicationListDto>()).ToList();

        return new PagedResult<MedicationListDto>
        {
            Items = items,
            TotalCount = total
        };
    }

    public async Task<IEnumerable<MedicationScheduleItemDto>> GetScheduleForUserAsync(int userId, DateTime date)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<MedicationScheduleItemDto>("GetMedicationScheduleForUser",
            new { UserId = userId, Date = date }, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> GetActiveMedicationsCountAsync(int userId)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>("GetActiveMedicationsCountForUser",
            new { UserId = userId }, commandType: CommandType.StoredProcedure);
    }
}
