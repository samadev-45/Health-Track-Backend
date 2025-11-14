using AutoMapper;
using Health.Application.Common;
using Health.Application.DTOs;
using Health.Application.DTOs.Common;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationWriteRepository _consultationRepo;
        private readonly IConsultationReadRepository _consultationReadRepo;
        private readonly IAppointmentWriteRepository _appointmentRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IPdfGenerator _pdfGenerator;
        private readonly IEmailSenderService _emailSender;
        private readonly IHttpContextAccessor _contextAccessor;

        public ConsultationService(
            IConsultationWriteRepository consultationRepo,
            IConsultationReadRepository consultationReadRepo,
            IAppointmentWriteRepository appointmentRepo,
            IUserRepository userRepo,
            IMapper mapper,
            IPdfGenerator pdfGenerator,
            IEmailSenderService emailSender,
            IHttpContextAccessor contextAccessor)
        {
            _consultationRepo = consultationRepo;
            _consultationReadRepo = consultationReadRepo;
            _appointmentRepo = appointmentRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _pdfGenerator = pdfGenerator;
            _emailSender = emailSender;
            _contextAccessor = contextAccessor;
        }

        // ------------------------------------------------------------
        // HELPER: Get current user ID from JWT
        // ------------------------------------------------------------
        private int GetCurrentUserId()
        {
            var userId = _contextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User not logged in.");

            return int.Parse(userId);
        }

        // ------------------------------------------------------------
        // HELPER: Check doctor ownership
        // ------------------------------------------------------------
        private void EnsureDoctorOwnership(int assignedDoctorId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId != assignedDoctorId)
                throw new UnauthorizedAccessException("You are not the assigned doctor for this consultation.");
        }

        // ------------------------------------------------------------
        // HELPER: Check patient ownership
        // ------------------------------------------------------------
        private void EnsurePatientOwnership(int assignedPatientId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId != assignedPatientId)
                throw new UnauthorizedAccessException("You are not the owner of this consultation.");
        }

        // ------------------------------------------------------------
        // 1. Create Consultation (Doctor Only)
        // ------------------------------------------------------------
        public async Task<ConsultationResponseDto> CreateConsultationAsync(
    int appointmentId,
    ConsultationCreateDto dto,
    CancellationToken ct = default)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment.Status != AppointmentStatus.Completed)
                throw new InvalidOperationException("Consultation can only be created after appointment completion.");

            EnsureDoctorOwnership(appointment.DoctorId);

            var consultation = new Consultation
            {
                AppointmentId = appointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                ChiefComplaint = dto.ChiefComplaint,
                Diagnosis = dto.Diagnosis,
                Advice = dto.Advice,
                DoctorNotes = dto.DoctorNotes,
                HealthValues = dto.HealthValues,   // ✅ Proper JSON object
                FollowUpDate = dto.FollowUpDate,
                Status = ConsultationStatus.Draft,
                CreatedOn = DateTime.UtcNow
            };

            await _consultationRepo.AddAsync(consultation);
            return _mapper.Map<ConsultationResponseDto>(consultation);
        }


        // ------------------------------------------------------------
        // 2. Get Consultations (Doctor)
        // ------------------------------------------------------------
        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByDoctorAsync(
            int doctorId,
            int? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10)
        {
            // ✔ Doctor can only view their own list
            EnsureDoctorOwnership(doctorId);

            return await _consultationReadRepo.GetConsultationsByDoctorAsync(
                doctorId, status, fromDate, toDate, page, pageSize);
        }

        // ------------------------------------------------------------
        // 3. Get Consultations (Patient)
        // ------------------------------------------------------------
        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByPatientAsync(
            int patientId,
            int? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10)
        {
            // ✔ Patient can only view their own list
            EnsurePatientOwnership(patientId);

            return await _consultationReadRepo.GetConsultationsByPatientAsync(
                patientId, status, fromDate, toDate, page, pageSize);
        }

        // ------------------------------------------------------------
        // 4. Get Consultation Details
        // ------------------------------------------------------------
        public async Task<ConsultationDetailsDto?> GetConsultationDetailsAsync(
            int consultationId,
            CancellationToken ct = default)
        {
            var details = await _consultationReadRepo.GetConsultationDetailsAsync(consultationId);
            if (details == null)
                return null;

            // Ownership validation
            int currentUserId = GetCurrentUserId();

            bool isDoctorOwner = details.DoctorId == currentUserId;
            bool isPatientOwner = details.PatientId == currentUserId;

            if (!(isDoctorOwner || isPatientOwner))
                throw new UnauthorizedAccessException("You are not allowed to view this consultation.");

            return details;
        }

        // ------------------------------------------------------------
        // 5. Upload Attachment (Doctor or Patient)
        // ------------------------------------------------------------
        public async Task<FileDto> UploadAttachmentAsync(
            int consultationId,
            UploadFileDto dto,
            CancellationToken ct = default)
        {
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            int currentUserId = GetCurrentUserId();
            bool isDoctor = _contextAccessor.HttpContext.User.IsInRole("Doctor");
            bool isPatient = _contextAccessor.HttpContext.User.IsInRole("Patient");

            if (isDoctor && currentUserId != consultation.DoctorId)
                throw new UnauthorizedAccessException("You are not the assigned doctor.");

            if (isPatient && currentUserId != consultation.PatientId)
                throw new UnauthorizedAccessException("You cannot upload for another patient.");

            byte[] bytes = Convert.FromBase64String(dto.Base64Data);

            var file = new FileStorage
            {
                ConsultationId = consultationId,
                FileName = dto.FileName,
                FileExtension = Path.GetExtension(dto.FileName),
                ContentType = dto.ContentType,
                FileData = bytes,
                FileSize = dto.FileSize,
                UploadedByUserId = currentUserId,
                CreatedOn = DateTime.UtcNow
            };

            await _consultationRepo.AddFileAsync(file, ct);

            return new FileDto
            {
                FileStorageId = file.FileStorageId,
                FileName = file.FileName,
                FileExtension = file.FileExtension,
                FileSize = file.FileSize,
                ContentType = file.ContentType,
                CreatedOn = file.CreatedOn
            };
        }

        // ------------------------------------------------------------
        // 6. Finalize Consultation (Doctor Only)
        // ------------------------------------------------------------
        public async Task<ConsultationResponseDto> FinalizeConsultationAsync(
            int consultationId,
            CancellationToken ct = default)
        {
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            //  Only the assigned doctor
            EnsureDoctorOwnership(consultation.DoctorId);

            if (consultation.Status == ConsultationStatus.Finalized)
                throw new InvalidOperationException("Consultation already finalized.");

            var prescription = consultation.Prescriptions
                ?.OrderByDescending(p => p.CreatedOn).FirstOrDefault();

            if (prescription == null)
            {
                prescription = new Prescription
                {
                    ConsultationId = consultation.ConsultationId,
                    CreatedBy = consultation.DoctorId,
                    CreatedOn = DateTime.UtcNow
                };

                await _consultationRepo.AddPrescriptionAsync(prescription, ct);
            }

            var patient = await _userRepo.GetByIdAsync(consultation.PatientId, ct);
            var doctor = await _userRepo.GetByIdAsync(consultation.DoctorId, ct);

            var items = prescription.Items ?? Array.Empty<PrescriptionItem>();
            var pdfModel = new PrescriptionPdfModel
            {
                PatientName = patient?.FullName ?? "",
                DoctorName = doctor?.FullName ?? "",
                Date = DateTime.UtcNow,
                Diagnosis = consultation.Diagnosis ?? "",
                Medicines = items.Select(pi => new PrescriptionPdfMedicineItem
                {
                    Name = pi.Medicine,
                    Dose = pi.Dose,
                    Frequency = pi.Frequency,
                    Duration = pi.DurationDays?.ToString() ?? "",
                    Notes = pi.Notes
                }).ToList(),
                Notes = consultation.Advice ?? "",
                Signature = ""
            };

            byte[] pdfBytes = await _pdfGenerator.GeneratePrescriptionPdfAsync(pdfModel, ct);

            var pdfFile = new FileStorage
            {
                ConsultationId = consultation.ConsultationId,
                FileName = $"prescription_{consultation.ConsultationId}.pdf",
                FileExtension = ".pdf",
                FileData = pdfBytes,
                FileSize = pdfBytes.Length,
                ContentType = "application/pdf",
                UploadedByUserId = consultation.DoctorId,
                CreatedOn = DateTime.UtcNow
            };

            await _consultationRepo.AddFileAsync(pdfFile, ct);

            consultation.IsPrescriptionGenerated = true;
            consultation.Status = ConsultationStatus.Finalized;
            consultation.ModifiedOn = DateTime.UtcNow;
            consultation.ModifiedBy = consultation.DoctorId;

            await _consultationRepo.UpdateAsync(consultation, ct);

            // Optional: email patient
            try
            {
                if (!string.IsNullOrEmpty(patient?.Email))
                {
                    await _emailSender.SendEmailAsync(
                        patient.Email,
                        "Your Prescription is Ready",
                        $"Hello {patient.FullName}, your prescription has been generated."
                    );
                }
            }
            catch { }

            return _mapper.Map<ConsultationResponseDto>(consultation);
        }

        // ------------------------------------------------------------
        // 7. Update Consultation (not implemented)
        // ------------------------------------------------------------
        public async Task<ConsultationResponseDto> UpdateConsultationAsync(
    int consultationId,
    ConsultationUpdateDto dto,
    CancellationToken ct = default)
        {
            // 1. Load consultation
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            // 2. Only the assigned doctor can update
            EnsureDoctorOwnership(consultation.DoctorId);

            // 3. Only editable if status = Draft
            if (consultation.Status != ConsultationStatus.Draft)
                throw new InvalidOperationException("Only draft consultations can be edited.");

            // 4. Optional: enforce 24-hour editing window
            if (consultation.CreatedOn.AddHours(24) < DateTime.UtcNow)
                throw new InvalidOperationException("Consultation can no longer be edited (24-hour window expired).");

            // 5. Update editable fields
            consultation.Diagnosis = dto.Diagnosis ?? consultation.Diagnosis;
            consultation.Advice = dto.Advice ?? consultation.Advice;
            consultation.DoctorNotes = dto.DoctorNotes ?? consultation.DoctorNotes;
            consultation.HealthValues = dto.HealthValues ?? consultation.HealthValues;

            consultation.FollowUpDate = dto.FollowUpDate ?? consultation.FollowUpDate;

            consultation.ModifiedOn = DateTime.UtcNow;
            consultation.ModifiedBy = consultation.DoctorId;

            // 6. Save changes
            await _consultationRepo.UpdateAsync(consultation, ct);

            // 7. Return mapped response
            return _mapper.Map<ConsultationResponseDto>(consultation);
        }


        public async Task<FileDownloadDto> DownloadFileAsync(int fileId, CancellationToken ct = default)
        {
            var file = await _consultationRepo.GetFileByIdAsync(fileId, ct)
                ?? throw new InvalidOperationException("File not found.");

            // Ownership validation
            int currentUserId = GetCurrentUserId();
            bool isDoctor = _contextAccessor.HttpContext.User.IsInRole("Doctor");
            bool isPatient = _contextAccessor.HttpContext.User.IsInRole("Patient");

            if (isDoctor && file.Consultation?.DoctorId != currentUserId)
                throw new UnauthorizedAccessException("You are not the assigned doctor.");

            if (isPatient && file.Consultation?.PatientId != currentUserId)
                throw new UnauthorizedAccessException("You are not allowed to download this file.");

            return new FileDownloadDto
            {
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileBytes = file.FileData
            };
        }

    }
}
