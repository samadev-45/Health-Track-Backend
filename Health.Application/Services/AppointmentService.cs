//using Health.Application.DTOs;
//using Health.Application.DTOs.Appointments;
//using Health.Application.Interfaces;
//using Health.Application.Interfaces.Dapper;
//using Health.Application.Interfaces.EFCore;
//using Microsoft.EntityFrameworkCore;


//namespace Health.Application.Services
//{
//    public class AppointmentService : IAppointmentService
//    {
//        private readonly IAppointmentReadRepository _readRepo;
//        private readonly IAppointmentWriteRepository _writeRepo;

//        public AppointmentService(
//            IAppointmentReadRepository readRepo,
//            IAppointmentWriteRepository writeRepo)
//        {
//            _readRepo = readRepo;
//            _writeRepo = writeRepo;
//        }

//        //  Get appointments for Doctor (via Dapper)
//        public async Task<(IEnumerable<DoctorApptRowDto> Appointments, int TotalCount)> GetDoctorAppointmentsAsync(
//            int doctorId, int? status, int page, int pageSize, CancellationToken ct = default)
//        {
//            var (appointments, total) = await _readRepo.GetAppointmentsByDoctorAsync(doctorId, status, page, pageSize, ct);

//            var rows = appointments.Select(a => new DoctorApptRowDto
//            {
//                AppointmentId = a.AppointmentId,
//                PatientId = a.PatientId,
//                AppointmentDate = a.AppointmentDate,
//                AppointmentTime = a.AppointmentTime,
//                Status = (int)a.Status,
//                PatientNotes = a.PatientNotes,
//                DoctorNotes = a.DoctorNotes,
//                RejectionReason = a.RejectionReason,
//                FollowUpDate = a.FollowUpDate,
//                Hospital = a.Hospital,
//                Location = a.Location,
//                PatientName = a.Patient?.FullName ?? string.Empty
//            });

//            return (rows, total);
//        }

//        //  Get appointments for Patient (via Dapper)
//        public async Task<(IEnumerable<PatientApptRowDto> Appointments, int TotalCount)> GetPatientAppointmentsAsync(
//            int patientId, int? status, int page, int pageSize, CancellationToken ct = default)
//        {
//            var (appointments, total) = await _readRepo.GetAppointmentsByPatientAsync(patientId, status, page, pageSize, ct);

//            var rows = appointments.Select(a => new PatientApptRowDto
//            {
//                AppointmentId = a.AppointmentId,
//                DoctorId = a.DoctorId,
//                AppointmentDate = a.AppointmentDate,
//                AppointmentTime = a.AppointmentTime,
//                Status = (int)a.Status,
//                PatientNotes = a.PatientNotes,
//                DoctorNotes = a.DoctorNotes,
//                RejectionReason = a.RejectionReason,
//                FollowUpDate = a.FollowUpDate,
//                Hospital = a.Hospital,
//                Location = a.Location,
//                DoctorName = a.Doctor?.FullName ?? string.Empty
//            });

//            return (rows, total);
//        }

//        //  Create new appointment (via EF Core)
//        public async Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default)
//        {
//            // Prevent duplicate bookings
//            var isDuplicate = await _writeRepo.IsDuplicateBookingAsync(patientId, dto.DoctorId, dto.AppointmentDate, dto.AppointmentTime, ct);
//            if (isDuplicate)
//                throw new InvalidOperationException("You already have an appointment for this time slot.");

//            // Delegate creation to EF-based repo
//            //return await _writeRepo.CreateAppointmentAsync(patientId, dto, ct);
//            try
//            {
//                return await _writeRepo.CreateAppointmentAsync(patientId, dto, ct);
//            }
//            catch (DbUpdateException dbEx)
//            {
//                throw new Exception(dbEx.InnerException?.Message ?? dbEx.Message);
//            }

//        }
//    }
//}

//Test 
using Health.Application.DTOs;
using Health.Application.DTOs.Appointments;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Health.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentReadRepository _readRepo;
        private readonly IAppointmentWriteRepository _writeRepo;
        private readonly IUserRepository _userRepo; // ✅ New abstraction

        public AppointmentService(
            IAppointmentReadRepository readRepo,
            IAppointmentWriteRepository writeRepo,
            IUserRepository userRepo)
        {
            _readRepo = readRepo;
            _writeRepo = writeRepo;
            _userRepo = userRepo;
        }

        // ✅ Get appointments for Doctor (via Dapper)
        public async Task<(IEnumerable<DoctorApptRowDto> Appointments, int TotalCount)> GetDoctorAppointmentsAsync(
            int doctorId, int? status, int page, int pageSize, CancellationToken ct = default)
        {
            var (appointments, total) = await _readRepo.GetAppointmentsByDoctorAsync(doctorId, status, page, pageSize, ct);

            var rows = appointments.Select(a => new DoctorApptRowDto
            {
                AppointmentId = a.AppointmentId,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                Status = (int)a.Status,
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

        // ✅ Get appointments for Patient (via Dapper)
        public async Task<(IEnumerable<PatientApptRowDto> Appointments, int TotalCount)> GetPatientAppointmentsAsync(
            int patientId, int? status, int page, int pageSize, CancellationToken ct = default)
        {
            var (appointments, total) = await _readRepo.GetAppointmentsByPatientAsync(patientId, status, page, pageSize, ct);

            var rows = appointments.Select(a => new PatientApptRowDto
            {
                AppointmentId = a.AppointmentId,
                DoctorId = a.DoctorId,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                Status = (int)a.Status,
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

        // ✅ Create new appointment (via EF Core Repository)
        public async Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default)
        {
            // 🔹 1. Validate patient
            var patient = await _userRepo.GetByIdAsync(patientId, ct);
            if (patient == null || patient.IsDeleted)
                throw new InvalidOperationException("Patient not found or inactive.");

             
            var doctor = await _userRepo.GetByIdAsync(dto.DoctorId, ct);
            if (doctor is null)
                throw new InvalidOperationException("Doctor not found.");
            if (doctor == null || doctor.IsDeleted || doctor.Role != RoleType.Doctor)
                throw new InvalidOperationException("Doctor not found or inactive.");





            // 🔹 3. Prevent duplicate bookings
            var isDuplicate = await _writeRepo.IsDuplicateBookingAsync(
                patientId, dto.DoctorId, dto.AppointmentDate, dto.AppointmentTime, ct);
            if (isDuplicate)
                throw new InvalidOperationException("You already have an appointment for this time slot.");

            // 🔹 4. Proceed with creation
            try
            {
                return await _writeRepo.CreateAppointmentAsync(patientId, dto, ct);
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                throw new Exception($"Database error while creating appointment: {inner}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unexpected error while creating appointment: {ex.Message}");
            }
        }
    }
}
