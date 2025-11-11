using Health.Application.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IConsultationService
    {
        /// <summary>
        /// Create a new consultation record linked to an appointment.
        /// </summary>
        Task<ConsultationResponseDto> CreateConsultationAsync(
            int appointmentId,
            ConsultationCreateDto dto,
            CancellationToken ct = default);

        /// <summary>
        /// Get all consultations for a given patient (or user).
        /// </summary>
        Task<IEnumerable<ConsultationResponseDto>> GetConsultationsByUserAsync(
            int userId,
            CancellationToken ct = default);
    }
}
