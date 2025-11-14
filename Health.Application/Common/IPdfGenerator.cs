using Health.Application.DTOs.Prescription;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Common
{
    public interface IPdfGenerator
    {
        Task<byte[]> GeneratePrescriptionPdfAsync(PrescriptionPdfModel model, CancellationToken ct = default);
    }
}
