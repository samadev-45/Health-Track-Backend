using Health.Application.DTOs;
using Health.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IConsultationService
    {
        // Create new consultation
        Task<ConsultationResponseDto> CreateConsultationAsync(int appointmentId, ConsultationCreateDto dto, CancellationToken ct = default);

        // Update draft consultation
        Task<ConsultationResponseDto> UpdateConsultationAsync(int consultationId, ConsultationUpdateDto dto, CancellationToken ct = default);

        // Finalize consultation (lock + generate PDF)
        Task<ConsultationResponseDto> FinalizeConsultationAsync(int consultationId, CancellationToken ct = default);

        // Upload files (reports, PDFs, images)
        Task<FileDto> UploadAttachmentAsync(int consultationId, UploadFileDto dto, CancellationToken ct = default);

        // Get consultations (doctor view - paginated)
        Task<PagedResult<ConsultationListDto>> GetConsultationsByDoctorAsync(int doctorId, int? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10);

        // Get consultations (patient view - paginated)
        Task<PagedResult<ConsultationListDto>> GetConsultationsByPatientAsync(int patientId, int? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10);

        // Get full consultation details (consultation + prescription + files)
        Task<ConsultationDetailsDto?> GetConsultationDetailsAsync(int consultationId, CancellationToken ct = default);
        Task<FileDownloadDto> DownloadFileAsync(int fileId, CancellationToken ct = default);

    }
}
