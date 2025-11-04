using Health.Application.DTOs.Appointments;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.EFCore
{
    public interface IAppointmentWriteRepository
    {
        Task<bool> IsDuplicateBookingAsync(int patientId, int doctorId, DateTime date, TimeSpan time, CancellationToken ct = default);
        Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default);
        // (Later) UpdateStatusAsync(), CancelAsync(), etc.
    }
}
