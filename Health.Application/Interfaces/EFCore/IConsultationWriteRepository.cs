using Health.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.EFCore
{
    public interface IConsultationWriteRepository : IGenericRepository<Consultation>
    {
        Task<IEnumerable<Consultation>> GetByUserIdAsync(int userId, CancellationToken ct = default);

        Task AddFileAsync(FileStorage file, CancellationToken ct = default);
        Task AddPrescriptionAsync(Prescription prescription, CancellationToken ct = default);
        Task<Consultation?> GetByIdAsync(int consultationId, CancellationToken ct = default);
        Task UpdateAsync(Consultation consultation, CancellationToken ct = default);
        Task<FileStorage?> GetFileByIdAsync(int fileId, CancellationToken ct = default);

        // Final chosen members (from HEAD branch)
        Task AddPrescriptionItemAsync(PrescriptionItem item, CancellationToken ct = default);
        Task UpdatePrescriptionItemAsync(PrescriptionItem item, CancellationToken ct = default);
        Task DeletePrescriptionItemAsync(int prescriptionItemId, CancellationToken ct = default);
        Task<Prescription?> GetPrescriptionByConsultationIdAsync(int consultationId, CancellationToken ct = default);
        Task<PrescriptionItem?> GetPrescriptionItemByIdAsync(int itemId, CancellationToken ct = default);
        Task<Prescription?> GetPrescriptionByIdAsync(int prescriptionId, CancellationToken ct = default);
    }
}
