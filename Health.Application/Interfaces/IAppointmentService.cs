using Health.Application.DTOs;
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
    }
}
