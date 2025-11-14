using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Infrastructure.Repositories.EFCore
{
    public class ConsultationRepository : GenericRepository<Consultation>, IConsultationWriteRepository
    {
        private readonly HealthDbContext _context;

        public ConsultationRepository(HealthDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Consultation>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Consultations
                .Include(c => c.Appointment)
                .Where(c => c.PatientId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedOn)
                .ToListAsync(ct);
        }

        public async Task AddFileAsync(FileStorage file, CancellationToken ct = default)
        {
            await _context.FileStorages.AddAsync(file, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddPrescriptionAsync(Prescription prescription, CancellationToken ct = default)
        {
            await _context.Prescriptions.AddAsync(prescription, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<Consultation?> GetByIdAsync(int consultationId, CancellationToken ct = default)
        {
            return await _context.Consultations
                .Include(c => c.Prescriptions)
                .ThenInclude(p => p.Items)
                .Include(c => c.Patient)
                .Include(c => c.Doctor)
                .FirstOrDefaultAsync(c => c.ConsultationId == consultationId && !c.IsDeleted, ct);
        }

        public async Task UpdateAsync(Consultation consultation, CancellationToken ct = default)
        {
            _context.Consultations.Update(consultation);
            await _context.SaveChangesAsync(ct);
        }
        public async Task<FileStorage?> GetFileByIdAsync(int fileId, CancellationToken ct = default)
        {
            return await _context.FileStorages
                .Include(f => f.Consultation)
                .FirstOrDefaultAsync(f => f.FileStorageId == fileId && !f.IsDeleted, ct);
        }

    }
}
