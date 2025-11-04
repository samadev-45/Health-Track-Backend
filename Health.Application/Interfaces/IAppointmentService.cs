using Health.Application.DTOs;
using Health.Application.DTOs.Appointments;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<(IEnumerable<DoctorApptRowDto> Appointments, int TotalCount)> GetDoctorAppointmentsAsync(
            int doctorId, int? status, int page, int pageSize, CancellationToken ct = default);

        Task<(IEnumerable<PatientApptRowDto> Appointments, int TotalCount)> GetPatientAppointmentsAsync(
            int patientId, int? status, int page, int pageSize, CancellationToken ct = default);

        Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default);
    }
}
