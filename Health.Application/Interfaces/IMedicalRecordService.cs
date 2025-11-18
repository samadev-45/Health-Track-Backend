using Health.Application.DTOs.Common;
using Health.Application.DTOs.MedicalRecord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto, CancellationToken ct = default);

        Task<PagedResult<MedicalRecordListDto>> GetMyRecordsAsync(
            int page = 1,
            int pageSize = 20,
            int? recordTypeId = null,
            DateTime? from = null,
            DateTime? to = null,
            CancellationToken ct = default);

        Task<MedicalRecordDto?> GetByIdAsync(int recordId, CancellationToken ct = default);

        Task<bool> DeleteAsync(int recordId, CancellationToken ct = default);
    }
}      
