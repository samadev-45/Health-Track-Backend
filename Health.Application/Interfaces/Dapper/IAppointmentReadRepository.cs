using Health.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Interfaces.Dapper
{
    public interface IAppointmentReadRepository
    {
        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByDoctorAsync(
            int doctorId, int? status, int page, int pageSize, CancellationToken ct = default);

        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByPatientAsync(
            int patientId, int? status, int page, int pageSize, CancellationToken ct = default);

       
    }
}
