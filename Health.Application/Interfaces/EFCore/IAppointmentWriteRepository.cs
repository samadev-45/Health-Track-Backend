using Health.Application.DTOs.Appointments;
using Health.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.EFCore
{
    public interface IAppointmentWriteRepository
    {
        Task<bool> IsDuplicateBookingAsync(int patientId, int doctorId, DateTime date, TimeSpan time, CancellationToken ct = default);
        Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default);
        Task<Appointment?> GetByIdAsync(int appointmentId, CancellationToken ct = default);
        Task UpdateAppointmentAsync(Appointment appointment, CancellationToken ct = default);
        Task<bool> HasOverlappingAppointmentAsync(int doctorId, DateTime date, TimeSpan time, int excludeAppointmentId, CancellationToken ct = default);
        Task AddHistoryAsync(AppointmentHistory history, CancellationToken ct = default);

        
    }
}
