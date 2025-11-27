using Health.Application.DTOs.Appointment;
using Health.Application.DTOs.Doctor;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<(IEnumerable<DoctorApptRowDto> Appointments, int TotalCount)> GetDoctorAppointmentsAsync(
            int doctorId, int? status, int page, int pageSize,DateTime? date, CancellationToken ct = default);

        Task<(IEnumerable<PatientApptRowDto> Appointments, int TotalCount)> GetPatientAppointmentsAsync(
            int patientId, int? status, int page, int pageSize, CancellationToken ct = default);

        Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default);

        Task<AppointmentDto> RescheduleAppointmentAsync(
            int appointmentId,
            int patientId,
            RescheduleAppointmentDto dto,
            CancellationToken ct = default);

        Task<AppointmentDto> CancelAppointmentAsync(
            int appointmentId,
            int userId,
            CancelAppointmentDto dto,
            CancellationToken ct = default);

        Task<AppointmentDto> ReassignAppointmentAsync(
            int appointmentId,
            int adminId,
            ReassignAppointmentDto dto,
            CancellationToken ct = default);

        Task<AppointmentDto> CompleteAppointmentAsync(
           int appointmentId,
           int doctorId,
           CompleteAppointmentDto dto,
           CancellationToken ct = default);

        Task<AppointmentDto> RespondToAppointmentAsync(
            int appointmentId,
            int doctorId,
            RespondToAppointmentDto dto,
            CancellationToken ct = default);

        Task<IEnumerable<AppointmentHistoryDto>> GetAppointmentHistoryAsync(
            int appointmentId,
            CancellationToken ct = default);
        Task<DoctorAvailabilityDto> GetAvailableSlotsAsync(
    int doctorId,
    DateTime date,
    CancellationToken ct = default);
        

    }
}
