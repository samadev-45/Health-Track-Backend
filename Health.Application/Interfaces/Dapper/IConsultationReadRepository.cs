using Health.Application.DTOs;
using Health.Application.DTOs.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.Dapper
{
    public interface IConsultationReadRepository
    {
        Task<PagedResult<ConsultationListDto>> GetConsultationsByDoctorAsync(
            int doctorId,
            int? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10);
        Task<PagedResult<ConsultationListDto>> GetConsultationsByPatientAsync(int patientId, int? status = null, DateTime? fromDate = null, DateTime? toDate = null,
   int page = 1,
   int pageSize = 10);
        Task<ConsultationDetailsDto?> GetConsultationDetailsAsync(int consultationId);
        Task<PagedResult<FileDto>> GetConsultationFilesAsync(
   int consultationId,
   int page = 1,
   int pageSize = 20);
    }
}
