using Health.Application.DTOs.Appointment;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Health.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Infrastructure.Repositories.EFCore
{
    public class AppointmentWriteRepository : IAppointmentWriteRepository
    {
        private readonly HealthDbContext _context;

        public AppointmentWriteRepository(HealthDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsDuplicateBookingAsync(int patientId, int doctorId, DateTime date, TimeSpan time, CancellationToken ct = default)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.PatientId == patientId &&
                a.DoctorId == doctorId &&
                a.AppointmentDate == date &&
                a.AppointmentTime == time &&
                !a.IsDeleted, ct);
        }

        public async Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTime = dto.AppointmentTime,
                Status = AppointmentStatus.Pending, // Pending
                PatientNotes = dto.PatientNotes,
                Hospital = dto.Hospital,
                Location = dto.Location,
                CreatedBy = patientId,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = false
            };

            await _context.Appointments.AddAsync(appointment, ct);
            await _context.SaveChangesAsync(ct);
            return appointment.AppointmentId;
        }
        public async Task<Appointment?> GetByIdAsync(int appointmentId, CancellationToken ct = default)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && !a.IsDeleted, ct);
        }

        public async Task UpdateAppointmentAsync(Appointment appointment, CancellationToken ct = default)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddHistoryAsync(AppointmentHistory history, CancellationToken ct = default)
        {
            await _context.AppointmentHistories.AddAsync(history, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> HasOverlappingAppointmentAsync(int doctorId, DateTime date, TimeSpan time, int excludeAppointmentId, CancellationToken ct = default)
        {
            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate == date &&
                a.AppointmentTime == time &&
                a.AppointmentId != excludeAppointmentId &&
                !a.IsDeleted, ct);
        }

    }
}
