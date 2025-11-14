using AutoMapper;
using Health.Application.Common;
using Health.Application.DTOs;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.Consultation;
using Health.Application.DTOs.File;
using Health.Application.DTOs.Prescription;
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
        private readonly HealthMetricEngine _metricEngine;
        public ConsultationService(
            IConsultationWriteRepository consultationRepo,
            IConsultationReadRepository consultationReadRepo,
            IAppointmentWriteRepository appointmentRepo,
            IUserRepository userRepo,
            IMapper mapper,
            IPdfGenerator pdfGenerator,
            IEmailSenderService emailSender,
            IHttpContextAccessor contextAccessor,
            HealthMetricEngine metricEngine)
        {
            _consultationRepo = consultationRepo;
            _consultationReadRepo = consultationReadRepo;
            _appointmentRepo = appointmentRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _pdfGenerator = pdfGenerator;
            _emailSender = emailSender;
            _contextAccessor = contextAccessor;
            _metricEngine = metricEngine;
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
                HealthValues = dto.HealthValues,   
                FollowUpDate = dto.FollowUpDate,
                Status = ConsultationStatus.Draft,
                CreatedOn = DateTime.UtcNow
            };
            consultation.TrendSummary = _metricEngine.BuildTrendSummary(dto.HealthValues);

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
        // Doctor can only view their own list
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
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            EnsureDoctorOwnership(consultation.DoctorId);

            if (consultation.Status != ConsultationStatus.Draft)
                throw new InvalidOperationException("Only draft consultations can be edited.");

            if (consultation.CreatedOn.AddHours(24) < DateTime.UtcNow)
                throw new InvalidOperationException("Consultation can no longer be edited (24-hour window expired).");

            consultation.ChiefComplaint = dto.ChiefComplaint ?? consultation.ChiefComplaint;
            consultation.Diagnosis = dto.Diagnosis ?? consultation.Diagnosis;
            consultation.Advice = dto.Advice ?? consultation.Advice;
            consultation.DoctorNotes = dto.DoctorNotes ?? consultation.DoctorNotes;
            consultation.FollowUpDate = dto.FollowUpDate ?? consultation.FollowUpDate;

            if (dto.HealthValues != null)
            {
                consultation.HealthValues = dto.HealthValues;
                consultation.TrendSummary = _metricEngine.BuildTrendSummary(dto.HealthValues);
            }

            consultation.ModifiedOn = DateTime.UtcNow;
            consultation.ModifiedBy = consultation.DoctorId;

            await _consultationRepo.UpdateAsync(consultation, ct);

            return _mapper.Map<ConsultationResponseDto>(consultation);
        }




        public async Task<FileDownloadDto> DownloadFileAsync(int fileId, CancellationToken ct = default)
        {
            var file = await _consultationRepo.GetFileByIdAsync(fileId, ct)
                ?? throw new InvalidOperationException("File not found.");

            if (file.ConsultationId == null)
                throw new InvalidOperationException("File not linked to consultation.");

            var consultation = file.Consultation
                ?? await _consultationRepo.GetByIdAsync(file.ConsultationId.Value, ct);

            int currentUserId = GetCurrentUserId();
            bool isDoctor = _contextAccessor.HttpContext.User.IsInRole("Doctor");
            bool isPatient = _contextAccessor.HttpContext.User.IsInRole("Patient");

            if (isDoctor && consultation.DoctorId != currentUserId)
                throw new UnauthorizedAccessException("You are not the assigned doctor.");

            if (isPatient && consultation.PatientId != currentUserId)
                throw new UnauthorizedAccessException("You cannot download this file.");

            return new FileDownloadDto
            {
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileBytes = file.FileData
            };
        }

        //CreatePrescription
        public async Task<PrescriptionDto> CreateOrGetPrescriptionAsync(int consultationId, PrescriptionCreateDto dto, CancellationToken ct = default)
        {
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            EnsureDoctorOwnership(consultation.DoctorId);

            var existing = await _consultationRepo.GetPrescriptionByConsultationIdAsync(consultationId, ct);
            if (existing != null)
            {
                // Update notes if provided
                if (!string.IsNullOrWhiteSpace(dto.Notes))
                {
                    existing.Notes = dto.Notes;
                    await _consultationRepo.UpdateAsync(consultation, ct); // if you store prescription changes through context save, else save directly
                }
                return _mapper.Map<PrescriptionDto>(existing);
            }

            var prescription = new Prescription
            {
                ConsultationId = consultationId,
                CreatedByUserId = GetCurrentUserId(),
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _consultationRepo.AddPrescriptionAsync(prescription, ct);

            // add items if any
            if (dto.Items != null && dto.Items.Any())
            {
                foreach (var itemDto in dto.Items)
                {
                    var item = _mapper.Map<PrescriptionItem>(itemDto);
                    item.PrescriptionId = prescription.PrescriptionId;
                    await _consultationRepo.AddPrescriptionItemAsync(item, ct);
                }

                // reload prescription with items
                prescription = await _consultationRepo.GetPrescriptionByConsultationIdAsync(consultationId, ct) ?? prescription;
            }

            return _mapper.Map<PrescriptionDto>(prescription);
        }

        public async Task<PrescriptionDto?> GetPrescriptionByConsultationAsync(int consultationId, CancellationToken ct = default)
        {
            var prescription = await _consultationRepo.GetPrescriptionByConsultationIdAsync(consultationId, ct);
            if (prescription == null) return null;

            // Ownership
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            EnsureDoctorOwnership(consultation.DoctorId);

            return _mapper.Map<PrescriptionDto>(prescription);
        }

        public async Task<PrescriptionItemDto> AddPrescriptionItemAsync(int prescriptionId, PrescriptionItemCreateDto dto, CancellationToken ct = default)
        {
            // Load prescription
            var prescription = await _consultationRepo.GetByIdAsync((await _consultationRepo.GetPrescriptionByConsultationIdAsync(0, ct))?.ConsultationId ?? 0, ct);
            // simpler: load by prescriptionId via repo (you may implement GetPrescriptionByIdAsync if preferred)

            // For clarity implement GetPrescriptionByIdAsync in repo — else you can query Prescription directly from context

            var pres = await _consultationRepo.GetPrescriptionByIdAsync(prescriptionId, ct)
                ?? throw new InvalidOperationException("Prescription not found.");

            // ensure ownership via consultation
            var consultation = pres.Consultation ?? await _consultationRepo.GetByIdAsync(pres.ConsultationId, ct);
            EnsureDoctorOwnership(consultation.DoctorId);

            var item = _mapper.Map<PrescriptionItem>(dto);
            item.PrescriptionId = prescriptionId;

            await _consultationRepo.AddPrescriptionItemAsync(item, ct);

            return _mapper.Map<PrescriptionItemDto>(item);
        }

        public async Task<PrescriptionItemDto> UpdatePrescriptionItemAsync(int itemId, PrescriptionItemUpdateDto dto, CancellationToken ct = default)
        {
            var item = await _consultationRepo.GetPrescriptionItemByIdAsync(itemId, ct)
                ?? throw new InvalidOperationException("Prescription item not found.");

            var prescription = item.Prescription ?? await _consultationRepo.GetPrescriptionByIdAsync(item.PrescriptionId, ct);
            var consultation = prescription.Consultation ?? await _consultationRepo.GetByIdAsync(prescription.ConsultationId, ct);

            EnsureDoctorOwnership(consultation.DoctorId);

            item.Medicine = dto.Medicine ?? item.Medicine;
            item.Strength = dto.Strength ?? item.Strength;
            item.Dose = dto.Dose ?? item.Dose;
            item.Frequency = dto.Frequency ?? item.Frequency;
            item.DurationDays = dto.DurationDays ?? item.DurationDays;
            item.Route = dto.Route ?? item.Route;
            item.Notes = dto.Notes ?? item.Notes;

            await _consultationRepo.UpdatePrescriptionItemAsync(item, ct);

            return _mapper.Map<PrescriptionItemDto>(item);
        }
        public async Task DeletePrescriptionItemAsync(int itemId, CancellationToken ct = default)
        {
            var item = await _consultationRepo.GetPrescriptionItemByIdAsync(itemId, ct)
                ?? throw new InvalidOperationException("Prescription item not found.");

            var prescription = item.Prescription ?? await _consultationRepo.GetPrescriptionByIdAsync(item.PrescriptionId, ct);
            var consultation = prescription.Consultation ?? await _consultationRepo.GetByIdAsync(prescription.ConsultationId, ct);

            EnsureDoctorOwnership(consultation.DoctorId);

            await _consultationRepo.DeletePrescriptionItemAsync(itemId, ct);
        }

    }
}
