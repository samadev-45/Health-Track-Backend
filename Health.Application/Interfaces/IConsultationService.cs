using System.Threading.Tasks;
using Health.Domain.Entities;

namespace Health.Application.Interfaces
{
    public interface IConsultationService
    {
        Task<Consultation> CreateConsultationAsync(int userId, string healthValuesJson);
    }
}
