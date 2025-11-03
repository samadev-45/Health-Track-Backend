using Health.Application.DTOs;
using Health.Application.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Health.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<DoctorApptRowDto> Appointments, int TotalCount)> GetDoctorAppointmentsAsync(
            int doctorId, int? status, int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (appointments, total) = await _repo.GetAppointmentsByDoctorAsync(doctorId, status, page, pageSize);

            var rows = appointments.Select(a => new DoctorApptRowDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                Status = Convert.ToInt32(a.Status),
                PatientNotes = a.PatientNotes,
                DoctorNotes = a.DoctorNotes,
                RejectionReason = a.RejectionReason,
                FollowUpDate = a.FollowUpDate,
                Hospital = a.Hospital,
                Location = a.Location,
                PatientName = a.Patient?.FullName ?? string.Empty
            });

            return (rows, total);
        }

        public async Task<(IEnumerable<PatientApptRowDto> Appointments, int TotalCount)> GetPatientAppointmentsAsync(
            int patientId, int? status, int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (appointments, total) = await _repo.GetAppointmentsByPatientAsync(patientId, status, page, pageSize);

            var rows = appointments.Select(a => new PatientApptRowDto
            {
                AppointmentId = a.AppointmentId,
                DoctorId = a.DoctorId,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                Status = Convert.ToInt32(a.Status),
                PatientNotes = a.PatientNotes,
                DoctorNotes = a.DoctorNotes,
                RejectionReason = a.RejectionReason,
                FollowUpDate = a.FollowUpDate,
                Hospital = a.Hospital,
                Location = a.Location,
                DoctorName = a.Doctor?.FullName ?? string.Empty
            });

            return (rows, total);
        }
    }
}
