using Health.Application.DTOs.Common;
using Health.Application.DTOs.Medication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IMedicationService
    {
        Task<MedicationDto> AddMedicationAsync(MedicationCreateDto dto, CancellationToken ct = default);
        Task<MedicationDto> UpdateMedicationAsync(int id, MedicationUpdateDto dto, CancellationToken ct = default);
        Task<bool> DeleteMedicationAsync(int id, CancellationToken ct = default);
        Task<PagedResult<MedicationListDto>> GetMyMedicationsAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
        Task<IEnumerable<MedicationScheduleItemDto>> GetScheduleForTodayAsync(CancellationToken ct = default);
        Task<MedicationDto?> GetByIdAsync(int id, CancellationToken ct = default);








    }
}
