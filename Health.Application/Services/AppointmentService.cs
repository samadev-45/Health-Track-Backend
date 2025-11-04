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
using AutoMapper;
using Health.Application.Configuration;
using Health.Application.DTOs;
using Health.Application.DTOs.Appointments;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        private readonly IUserRepository _userRepo;
        private readonly AppointmentPolicyConfig _policyConfig;
        private readonly IMapper _mapper;
        public AppointmentService(
            IAppointmentReadRepository readRepo,
            IAppointmentWriteRepository writeRepo,
            IUserRepository userRepo,
            IOptions<AppointmentPolicyConfig> policyConfig, IMapper mapper)
        {
            _readRepo = readRepo;
            _writeRepo = writeRepo;
            _userRepo = userRepo;
            _policyConfig = policyConfig.Value;
            _mapper = mapper;
        }

        //  Get appointments for Doctor (via Dapper)
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

        //  Get appointments for Patient (via Dapper)
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

        //  Create new appointment (via EF Core)
        public async Task<int> CreateAppointmentAsync(int patientId, CreateAppointmentDto dto, CancellationToken ct = default)
        {
            var patient = await _userRepo.GetByIdAsync(patientId, ct);
            if (patient == null || patient.IsDeleted)
                throw new InvalidOperationException("Patient not found or inactive.");

            var doctor = await _userRepo.GetByIdAsync(dto.DoctorId, ct);
            if (doctor == null || doctor.IsDeleted || doctor.Role != RoleType.Doctor)
                throw new InvalidOperationException("Doctor not found or inactive.");

            var isDuplicate = await _writeRepo.IsDuplicateBookingAsync(
                patientId, dto.DoctorId, dto.AppointmentDate, dto.AppointmentTime, ct);
            if (isDuplicate)
                throw new InvalidOperationException("You already have an appointment for this time slot.");

            try
            {
                return await _writeRepo.CreateAppointmentAsync(patientId, dto, ct);
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(dbEx.InnerException?.Message ?? dbEx.Message);
            }
        }

        //   Reschedule existing appointment
        public async Task<AppointmentDto> RescheduleAppointmentAsync(int appointmentId, int patientId, RescheduleAppointmentDto dto, CancellationToken ct = default)

        {
            //  Load appointment
            var appointment = await _writeRepo.GetByIdAsync(appointmentId, ct);
            if (appointment == null || appointment.IsDeleted)
                throw new InvalidOperationException("Appointment not found.");

            // 2️⃣ Ownership check
            if (appointment.PatientId != patientId)
                throw new UnauthorizedAccessException("You cannot modify another patient's appointment.");

            // 3️⃣ State validation
            if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Only pending or confirmed appointments can be rescheduled.");

            // 4️⃣ Policy cutoff check
            var now = DateTime.UtcNow;
            var appointmentStart = appointment.AppointmentDate.Add(appointment.AppointmentTime);
            if (appointmentStart.Subtract(now).TotalHours < _policyConfig.RescheduleCutoffHours)
                throw new InvalidOperationException($"Reschedule is allowed only before {_policyConfig.RescheduleCutoffHours} hours of appointment time.");

            // 5️⃣ Max reschedule check — (load from history)
            // (You can later move this to a dedicated history repository if needed)
            var rescheduleCount = appointment.History?.Count(h => h.Action == "Rescheduled") ?? 0;
            if (rescheduleCount >= _policyConfig.MaxReschedules)
                throw new InvalidOperationException("You have reached the maximum number of reschedules.");

            // 6️⃣ Check overlapping appointment
            bool isOverlap = await _writeRepo.HasOverlappingAppointmentAsync(
                appointment.DoctorId,
                dto.NewAppointmentDate,
                dto.NewAppointmentTime,
                appointment.AppointmentId,
                ct);
            if (isOverlap)
                throw new InvalidOperationException("The doctor is not available for the selected time.");

            // 7️⃣ Update fields
            appointment.AppointmentDate = dto.NewAppointmentDate;
            appointment.AppointmentTime = dto.NewAppointmentTime;
            appointment.Status = AppointmentStatus.Pending;
            appointment.ModifiedOn = DateTime.UtcNow;
            appointment.ModifiedBy = patientId;

            await _writeRepo.UpdateAppointmentAsync(appointment, ct);

            // 8️⃣ Log history
            var history = new AppointmentHistory
            {
                AppointmentId = appointment.AppointmentId,
                Action = "Rescheduled",
                ChangedByUserId = patientId,
                ChangedAt = DateTime.UtcNow,
                Reason = dto.Reason,
                OldAppointmentDate = appointment.AppointmentDate,
                NewAppointmentDate = dto.NewAppointmentDate
            };
            await _writeRepo.AddHistoryAsync(history, ct);


            // 9️⃣ (Later) Send notification to doctor
            return _mapper.Map<AppointmentDto>(appointment);
            ;
        }
        

    }
}

