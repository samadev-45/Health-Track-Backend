using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
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

        // -----------------------------------------------------
        // PRESCRIPTION ITEM OPERATIONS (kept from HEAD)
        // -----------------------------------------------------

        public async Task AddPrescriptionItemAsync(PrescriptionItem item, CancellationToken ct = default)
        {
            await _context.PrescriptionItems.AddAsync(item, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdatePrescriptionItemAsync(PrescriptionItem item, CancellationToken ct = default)
        {
            _context.PrescriptionItems.Update(item);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeletePrescriptionItemAsync(int prescriptionItemId, CancellationToken ct = default)
        {
            var item = await _context.PrescriptionItems
                .FirstOrDefaultAsync(i => i.PrescriptionItemId == prescriptionItemId && !i.IsDeleted, ct);

            if (item == null) return;

            item.IsDeleted = true;
            item.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
        }

        public async Task<Prescription?> GetPrescriptionByConsultationIdAsync(int consultationId, CancellationToken ct = default)
        {
            return await _context.Prescriptions
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.ConsultationId == consultationId && !p.IsDeleted, ct);
        }

        public async Task<PrescriptionItem?> GetPrescriptionItemByIdAsync(int itemId, CancellationToken ct = default)
        {
            return await _context.PrescriptionItems
                .FirstOrDefaultAsync(i => i.PrescriptionItemId == itemId && !i.IsDeleted, ct);
        }

        public async Task<Prescription?> GetPrescriptionByIdAsync(int prescriptionId, CancellationToken ct = default)
        {
            return await _context.Prescriptions
                .Include(p => p.Items)
                .Include(p => p.Consultation)
                .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId && !p.IsDeleted, ct);
        }
    }
}
