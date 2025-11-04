using Health.Application.DTOs.Appointments;
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
    }
}
