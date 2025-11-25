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

        // -------------------- Helpers --------------------

        private int GetCurrentUserId()
        {
            var userId = _contextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("User not logged in.");

            return int.Parse(userId);
        }

        private bool CurrentUserIsInRole(string role)
        {
            return _contextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        private void EnsureDoctorOwnership(int assignedDoctorId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId != assignedDoctorId && !CurrentUserIsInRole("Admin"))
                throw new UnauthorizedAccessException("You are not the assigned doctor for this resource.");
        }

        private void EnsurePatientOwnership(int assignedPatientId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId != assignedPatientId && !CurrentUserIsInRole("Admin"))
                throw new UnauthorizedAccessException("You are not the owner of this resource.");
        }

        
        private void EnsureDoctorOrPatientOwnership(int assignedDoctorId, int assignedPatientId)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId != assignedDoctorId && currentUserId != assignedPatientId && !CurrentUserIsInRole("Admin"))
                throw new UnauthorizedAccessException("You are not authorized to access this consultation.");
        }

        public async Task<ConsultationResponseDto> CreateConsultationAsync(
            int appointmentId,
            ConsultationCreateDto dto,
            CancellationToken ct = default)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment.Status != AppointmentStatus.Completed)
                throw new InvalidOperationException("Consultation can only be created after appointment completion.");

            // Only assigned doctor can create a consultation for the appointment (or Admin)
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
                CreatedOn = DateTime.UtcNow,
                CreatedBy = GetCurrentUserId()
            };

            consultation.TrendSummary = _metricEngine.BuildTrendSummary(dto.HealthValues);

            await _consultationRepo.AddAsync(consultation);

            return _mapper.Map<ConsultationResponseDto>(consultation);
        }

  
        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByDoctorAsync(
            int doctorId,
            int? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10)
        {
           
            EnsureDoctorOwnership(doctorId);

            return await _consultationReadRepo.GetConsultationsByDoctorAsync(
                doctorId, status, fromDate, toDate, page, pageSize);
        }

        
        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByPatientAsync(
            int patientId,
            int? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10)
        {
           
            EnsurePatientOwnership(patientId);

            return await _consultationReadRepo.GetConsultationsByPatientAsync(
                patientId, status, fromDate, toDate, page, pageSize);
        }

        
        public async Task<ConsultationDetailsDto?> GetConsultationDetailsAsync(
            int consultationId,
            CancellationToken ct = default)
        {
            var details = await _consultationReadRepo.GetConsultationDetailsAsync(consultationId);
            if (details == null)
                return null;

            // Ownership validation: doctor or patient or admin
            EnsureDoctorOrPatientOwnership(details.DoctorId, details.PatientId);

            return details;
        }

      
        public async Task<FileDto> UploadAttachmentAsync(
            int consultationId,
            UploadFileDto dto,
            CancellationToken ct = default)
        {
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            int currentUserId = GetCurrentUserId();
            bool isDoctor = CurrentUserIsInRole("Doctor");
            bool isPatient = CurrentUserIsInRole("Patient");

            if (isDoctor && currentUserId != consultation.DoctorId && !CurrentUserIsInRole("Admin"))
                throw new UnauthorizedAccessException("You are not the assigned doctor.");

            if (isPatient && currentUserId != consultation.PatientId && !CurrentUserIsInRole("Admin"))
                throw new UnauthorizedAccessException("You cannot upload for another patient.");

            byte[] bytes = Convert.FromBase64String(dto.Base64Data ?? string.Empty);

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

     
        public async Task<ConsultationResponseDto> FinalizeConsultationAsync(
            int consultationId,
            CancellationToken ct = default)
        {
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            // Only the assigned doctor (or Admin) can finalize
            EnsureDoctorOwnership(consultation.DoctorId);

            if (consultation.Status == ConsultationStatus.Finalized)
                throw new InvalidOperationException("Consultation already finalized.");

            var prescription = consultation.Prescriptions?.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

            if (prescription == null)
            {
                prescription = new Prescription
                {
                    ConsultationId = consultation.ConsultationId,
                    CreatedByUserId = consultation.DoctorId,
                    CreatedAt = DateTime.UtcNow
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
            catch
            {
                // swallow email errors - not critical
            }

            return _mapper.Map<ConsultationResponseDto>(consultation);
        }

   
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

            if (dto.HealthValues != null)
            {
                consultation.HealthValues = dto.HealthValues;
                consultation.TrendSummary = _metricEngine.BuildTrendSummary(dto.HealthValues);
            }

            consultation.FollowUpDate = dto.FollowUpDate ?? consultation.FollowUpDate;
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

            var consultation = file.Consultation ?? await _consultationRepo.GetByIdAsync(file.ConsultationId.Value, ct);

            int currentUserId = GetCurrentUserId();
            bool isDoctor = CurrentUserIsInRole("Doctor");
            bool isPatient = CurrentUserIsInRole("Patient");

            if (isDoctor && consultation.DoctorId != currentUserId && !CurrentUserIsInRole("Admin"))
                throw new UnauthorizedAccessException("You are not the assigned doctor.");

            if (isPatient && consultation.PatientId != currentUserId && !CurrentUserIsInRole("Admin"))
                throw new UnauthorizedAccessException("You cannot download this file.");

            return new FileDownloadDto
            {
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                FileBytes = file.FileData
            };
        }

        // -

        public async Task<PrescriptionDto> CreateOrGetPrescriptionAsync(int consultationId, PrescriptionCreateDto dto, CancellationToken ct = default)
        {
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            
            EnsureDoctorOrPatientOwnership(consultation.DoctorId, consultation.PatientId);

            var existing = await _consultationRepo.GetPrescriptionByConsultationIdAsync(consultationId, ct);
            if (existing != null)
            {
                
                if (!string.IsNullOrWhiteSpace(dto.Notes))
                {
                    existing.Notes = dto.Notes;
                    
                    var updateconsultation = await _consultationRepo.GetByIdAsync(consultationId, ct);
                    await _consultationRepo.UpdateAsync(updateconsultation, ct);
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

            // Authorize: doctor, patient or admin
            var consultation = await _consultationRepo.GetByIdAsync(consultationId, ct)
                ?? throw new InvalidOperationException("Consultation not found.");

            EnsureDoctorOrPatientOwnership(consultation.DoctorId, consultation.PatientId);

            return _mapper.Map<PrescriptionDto>(prescription);
        }

        public async Task<PrescriptionItemDto> AddPrescriptionItemAsync(int prescriptionId, PrescriptionItemCreateDto dto, CancellationToken ct = default)
        {
            var pres = await _consultationRepo.GetPrescriptionByIdAsync(prescriptionId, ct)
                ?? throw new InvalidOperationException("Prescription not found.");

            var consultation = pres.Consultation ?? await _consultationRepo.GetByIdAsync(pres.ConsultationId, ct);

            // Only doctor (or admin) can add items — but if you want patients to request meds, adjust accordingly.
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

            var pres = item.Prescription ?? await _consultationRepo.GetPrescriptionByIdAsync(item.PrescriptionId, ct);
            var consultation = pres.Consultation ?? await _consultationRepo.GetByIdAsync(pres.ConsultationId, ct);

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

            var pres = item.Prescription ?? await _consultationRepo.GetPrescriptionByIdAsync(item.PrescriptionId, ct);
            var consultation = pres.Consultation ?? await _consultationRepo.GetByIdAsync(pres.ConsultationId, ct);

            EnsureDoctorOwnership(consultation.DoctorId);

            await _consultationRepo.DeletePrescriptionItemAsync(itemId, ct);
        }
    }
}
