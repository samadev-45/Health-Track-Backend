using System.Collections.Generic;
using System.Threading.Tasks;
using Health.Domain.Entities;

namespace Health.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        //return a tuple (appointments,Totalcount)
        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByDoctorAsync(int doctorId, int? status, int page, int pageSize);
        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByPatientAsync(int patientId, int? status, int page, int pageSize);
    }
}
