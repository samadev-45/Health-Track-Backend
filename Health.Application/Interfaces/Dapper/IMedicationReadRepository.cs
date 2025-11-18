using Health.Application.DTOs.Medication;
using Health.Application.DTOs.Common;

public interface IMedicationReadRepository
{
    Task<PagedResult<MedicationListDto>> GetForUserAsync(int userId, int page, int pageSize);
    Task<IEnumerable<MedicationScheduleItemDto>> GetScheduleForUserAsync(int userId, DateTime date);
    Task<int> GetActiveMedicationsCountAsync(int userId);
}
