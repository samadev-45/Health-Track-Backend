using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Health.Application.Interfaces;
using Health.Domain.Entities;
using Health.Infrastructure.Data;

namespace Health.Infrastructure.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly HealthDbContext _context;
        private readonly NormalRangeService _rangeService;

        public ConsultationService(HealthDbContext context, NormalRangeService rangeService)
        {
            _context = context;
            _rangeService = rangeService;
        }

        public async Task<Consultation> CreateConsultationAsync(int userId, string healthValuesJson)
        {
            var values = JsonSerializer.Deserialize<Dictionary<string, decimal>>(healthValuesJson);
            var abnormal = new Dictionary<string, decimal>();

            foreach (var kv in values!)
            {
                if (_rangeService.IsAbnormal(kv.Key, kv.Value))
                    abnormal[kv.Key] = kv.Value;
            }

            var consultation = new Consultation
            {
                UserId = userId,
                HealthValuesJson = JsonSerializer.Serialize(new
                {
                    Values = values,
                    Abnormal = abnormal
                }),
                CreatedAt = DateTime.UtcNow
            };

            _context.Consultations.Add(consultation);
            await _context.SaveChangesAsync();
            return consultation;
        }
    }
}
