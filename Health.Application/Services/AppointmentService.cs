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
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private readonly IEmailSenderService _emailSenderService;
        public AppointmentService(
            IAppointmentReadRepository readRepo,
            IAppointmentWriteRepository writeRepo,
            IUserRepository userRepo,
            IOptions<AppointmentPolicyConfig> policyConfig,
            IMapper mapper,
            IEmailSenderService emailSenderService)
        {
            _readRepo = readRepo;
            _writeRepo = writeRepo;
            _userRepo = userRepo;
            _policyConfig = policyConfig.Value;
            _mapper = mapper;
            _emailSenderService = emailSenderService;
        }

        #region Appointment Queries

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



        // Create Appointment

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
                // Create appointment
                var appointmentId = await _writeRepo.CreateAppointmentAsync(patientId, dto, ct);

                // Send notification to doctor
                await _emailSenderService.SendEmailAsync(
                    doctor.Email,
                    "New Appointment Request",
                    $"Hello Dr. {doctor.FullName},<br/>" +
                    $"Patient <b>{patient.FullName}</b> has booked an appointment with you on " +
                    $"<b>{dto.AppointmentDate:dd MMM yyyy}</b> at <b>{dto.AppointmentTime}</b>."
                );

                return appointmentId;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception(dbEx.InnerException?.Message ?? dbEx.Message);
            }
        }




        // Reschedule

        public async Task<AppointmentDto> RescheduleAppointmentAsync(int appointmentId, int patientId, RescheduleAppointmentDto dto, CancellationToken ct = default)
        {
            var appointment = await _writeRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment.PatientId != patientId)
                throw new UnauthorizedAccessException("You cannot modify another patient's appointment.");

            if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Only pending or confirmed appointments can be rescheduled.");

            var now = DateTime.UtcNow;
            var appointmentStart = appointment.AppointmentDate.Add(appointment.AppointmentTime);
            if (appointmentStart.Subtract(now).TotalHours < _policyConfig.RescheduleCutoffHours)
                throw new InvalidOperationException($"Reschedule is allowed only before {_policyConfig.RescheduleCutoffHours} hours of appointment time.");

            var rescheduleCount = appointment.History?.Count(h => h.Action == "Rescheduled") ?? 0;
            if (rescheduleCount >= _policyConfig.MaxReschedules)
                throw new InvalidOperationException("You have reached the maximum number of reschedules.");

            bool isOverlap = await _writeRepo.HasOverlappingAppointmentAsync(
                appointment.DoctorId,
                dto.NewAppointmentDate,
                dto.NewAppointmentTime,
                appointment.AppointmentId,
                ct);
            if (isOverlap)
                throw new InvalidOperationException("The doctor is not available for the selected time.");

            var oldDate = appointment.AppointmentDate;

            appointment.AppointmentDate = dto.NewAppointmentDate;
            appointment.AppointmentTime = dto.NewAppointmentTime;
            appointment.Status = AppointmentStatus.Pending;
            appointment.ModifiedOn = DateTime.UtcNow;
            appointment.ModifiedBy = patientId;

            await _writeRepo.UpdateAppointmentAsync(appointment, ct);

            await _writeRepo.AddHistoryAsync(new AppointmentHistory
            {
                AppointmentId = appointment.AppointmentId,
                Action = "Rescheduled",
                ChangedByUserId = patientId,
                ChangedAt = DateTime.UtcNow,
                Reason = dto.Reason,
                OldAppointmentDate = oldDate,
                NewAppointmentDate = dto.NewAppointmentDate
            }, ct);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        

        // Cancel

        public async Task<AppointmentDto> CancelAppointmentAsync(int appointmentId, int userId, CancelAppointmentDto dto, CancellationToken ct = default)
        {
            var appointment = await _writeRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment.Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.Rejected)
                throw new InvalidOperationException("Cannot cancel a completed, canceled, or rejected appointment.");

            var user = await _userRepo.GetByIdAsync(userId, ct)
                ?? throw new InvalidOperationException("User not found.");

            // Patient can cancel anytime
            bool isAdmin = user.Role == RoleType.Admin;
            bool isDoctor = user.Role == RoleType.Doctor;
            bool isPatient = user.Role == RoleType.Patient;

            if (isDoctor && appointment.DoctorId != userId)
                throw new UnauthorizedAccessException("You can cancel only your own appointments.");

            if (isPatient && appointment.PatientId != userId)
                throw new UnauthorizedAccessException("You can cancel only your own appointments.");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.RejectionReason = dto.Reason;
            appointment.ModifiedOn = DateTime.UtcNow;
            appointment.ModifiedBy = userId;

            await _writeRepo.UpdateAppointmentAsync(appointment, ct);

            await _writeRepo.AddHistoryAsync(new AppointmentHistory
            {
                AppointmentId = appointment.AppointmentId,
                Action = "Canceled",
                ChangedByUserId = userId,
                ChangedAt = DateTime.UtcNow,
                Reason = dto.Reason
            }, ct);

            // TODO: Notify relevant party (Doctor/Patient/Admin)

            return _mapper.Map<AppointmentDto>(appointment);
        }

        

         //Reassign (Admin Only)

        public async Task<AppointmentDto> ReassignAppointmentAsync(int appointmentId, int adminId, ReassignAppointmentDto dto, CancellationToken ct = default)
        {
            var appointment = await _writeRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment.Status != AppointmentStatus.Pending && appointment.Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Only pending or confirmed appointments can be reassigned.");

            var admin = await _userRepo.GetByIdAsync(adminId, ct)
                ?? throw new InvalidOperationException("Admin not found.");

            if (admin.Role != RoleType.Admin)
                throw new UnauthorizedAccessException("Only admins can reassign appointments.");

            var newDoctor = await _userRepo.GetByIdAsync(dto.NewDoctorId, ct)
                ?? throw new InvalidOperationException("New doctor not found.");

            bool overlap = await _writeRepo.HasOverlappingAppointmentAsync(
                dto.NewDoctorId,
                dto.NewAppointmentDate,
                dto.NewAppointmentTime,
                appointment.AppointmentId,
                ct);

            if (overlap)
                throw new InvalidOperationException("The new doctor is not available at the selected time.");

            appointment.DoctorId = dto.NewDoctorId;
            appointment.AppointmentDate = dto.NewAppointmentDate;
            appointment.AppointmentTime = dto.NewAppointmentTime;
            appointment.Status = AppointmentStatus.Pending;
            appointment.RejectionReason = "Reassigned by admin";
            appointment.ModifiedBy = adminId;
            appointment.ModifiedOn = DateTime.UtcNow;

            await _writeRepo.UpdateAppointmentAsync(appointment, ct);

            await _writeRepo.AddHistoryAsync(new AppointmentHistory
            {
                AppointmentId = appointment.AppointmentId,
                Action = "Reassigned",
                ChangedByUserId = adminId,
                ChangedAt = DateTime.UtcNow,
                Reason = dto.Reason
            }, ct);

            

            return _mapper.Map<AppointmentDto>(appointment);
        }
        #endregion
        public async Task<AppointmentDto> CompleteAppointmentAsync(
    int appointmentId,
    int doctorId,
    CompleteAppointmentDto dto,
    CancellationToken ct = default)
        {
            var appointment = await _writeRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            // Ownership check
            if (appointment.DoctorId != doctorId)
                throw new UnauthorizedAccessException("You can complete only your own appointments.");

            // Status must be Confirmed
            if (appointment.Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Only confirmed appointments can be marked as completed.");

            // Time validation — current UTC must be >= appointment start time
            var appointmentStart = appointment.AppointmentDate.Add(appointment.AppointmentTime);
            if (DateTime.UtcNow < appointmentStart)
                throw new InvalidOperationException("Appointment cannot be completed before its start time.");

            // Update fields
            appointment.Status = AppointmentStatus.Completed;
            appointment.DoctorNotes = dto.DoctorNotes;
            appointment.ModifiedBy = doctorId;
            appointment.ModifiedOn = DateTime.UtcNow;

            // Save and log history (transactional via repo)
            await _writeRepo.UpdateAppointmentAsync(appointment, ct);

            await _writeRepo.AddHistoryAsync(new AppointmentHistory
            {
                AppointmentId = appointment.AppointmentId,
                Action = "Completed",
                ChangedByUserId = doctorId,
                ChangedAt = DateTime.UtcNow,
                Reason = "Appointment completed by doctor"
            }, ct);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto> RespondToAppointmentAsync(
    int appointmentId,
    int doctorId,
    RespondToAppointmentDto dto,
    CancellationToken ct = default)
        {
            var appointment = await _writeRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment.DoctorId != doctorId)
                throw new UnauthorizedAccessException("You can only respond to your own appointments.");

            if (appointment.Status != AppointmentStatus.Pending)
                throw new InvalidOperationException("Only pending appointments can be accepted or rejected.");

            // Update status
            appointment.Status = dto.IsAccepted ? AppointmentStatus.Confirmed : AppointmentStatus.Rejected;
            appointment.RejectionReason = dto.IsAccepted ? null : dto.Remarks;
            appointment.DoctorNotes = dto.IsAccepted ? dto.Remarks : null;
            appointment.ModifiedBy = doctorId;
            appointment.ModifiedOn = DateTime.UtcNow;

            await _writeRepo.UpdateAppointmentAsync(appointment, ct);

            await _writeRepo.AddHistoryAsync(new AppointmentHistory
            {
                AppointmentId = appointment.AppointmentId,
                Action = dto.IsAccepted ? "Accepted" : "Rejected",
                ChangedByUserId = doctorId,
                ChangedAt = DateTime.UtcNow,
                Reason = dto.Remarks
            }, ct);

            // Send email notification
            if (dto.IsAccepted)
            {
                await _emailSenderService.SendEmailAsync(
                    appointment.Patient.Email,
                    "Appointment Confirmed",
                    $"Your appointment with Dr. {appointment.Doctor.FullName} on " +
                    $"{appointment.AppointmentDate:dd MMM yyyy} at {appointment.AppointmentTime} has been confirmed."
                );
            }
            else
            {
                await _emailSenderService.SendEmailAsync(
                    appointment.Patient.Email,
                    "Appointment Rejected",
                    $"Unfortunately, your appointment with Dr. {appointment.Doctor.FullName} on " +
                    $"{appointment.AppointmentDate:dd MMM yyyy} at {appointment.AppointmentTime} was rejected.<br/>" +
                    $"Reason: {dto.Remarks ?? "Not specified"}"
                );
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }



        public async Task<IEnumerable<AppointmentHistoryDto>> GetAppointmentHistoryAsync(int appointmentId, CancellationToken ct = default)
        {
            var logs = await _readRepo.GetAppointmentHistoryAsync(appointmentId, ct);
            return logs.Select(h => new AppointmentHistoryDto
            {
                Action = h.Action,
                Reason = h.Reason,
                ChangedBy = h.ChangedByUser?.FullName ?? "System",
                ChangedAt = h.ChangedAt
            });
        }



    }
}

