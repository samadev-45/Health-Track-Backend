using Health.Application.DTOs.Common;
using Health.Application.DTOs.HealthMetric;
using Health.Application.DTOs.MedicalRecord;

namespace Health.Application.Interfaces.Dapper
{
    public interface IMedicalRecordReadRepository
    {
        Task<PagedResult<MedicalRecordListDto>> GetForUserAsync(int userId, int page, int pageSize, int? recordTypeId, DateTime? from, DateTime? to);
        Task<MedicalRecordDto?> GetByIdAsync(int recordId);
        

    }
}
