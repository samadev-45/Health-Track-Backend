//using Health.Application.DTOs.Appointments;
//using Health.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Health.Application.Interfaces
//{
//    public interface IAppointmentRepository
//    {
//        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByDoctorAsync(
//            int doctorId, int? status, int page, int pageSize, CancellationToken ct = default);

//        Task<(IEnumerable<Appointment> Appointments, int TotalCount)> GetAppointmentsByPatientAsync(
//            int patientId, int? status, int page, int pageSize, CancellationToken ct = default);

//        Task<bool> IsDuplicateBookingAsync(int patientId, int doctorId, DateTime date, TimeSpan time, CancellationToken ct = default);

//        Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default);
//    }
//}
