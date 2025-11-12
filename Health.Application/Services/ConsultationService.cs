using AutoMapper;
using Health.Application.DTOs;
using Health.Application.DTOs.Common;
using Health.Application.Interfaces;
using Health.Application.Interfaces.Dapper;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Health.Domain.Enums;

namespace Health.Application.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationWriteRepository _consultationRepo;
        private readonly IConsultationReadRepository _consultationReadRepo;
        private readonly IAppointmentWriteRepository _appointmentRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public ConsultationService(
            IConsultationWriteRepository consultationRepo,
            IConsultationReadRepository consultationReadRepo,
            IAppointmentWriteRepository appointmentRepo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _consultationRepo = consultationRepo;
            _consultationReadRepo = consultationReadRepo;
            _appointmentRepo = appointmentRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        // ✅ EF-based write
        public async Task<ConsultationResponseDto> CreateConsultationAsync(int appointmentId, ConsultationCreateDto dto, CancellationToken ct = default)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment.Status != Domain.Enums.AppointmentStatus.Completed)
                throw new InvalidOperationException("Consultation can only be created after appointment completion.");

            var consultation = new Consultation
            {
                AppointmentId = appointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                DoctorNotes = dto.DoctorNotes,
                Diagnosis = dto.Diagnosis,
                Advice = dto.Advice,
                HealthValuesJson = dto.HealthValuesJson ?? "{}",
                FollowUpDate = dto.FollowUpDate,
                Status = ConsultationStatus.Draft
            };

            await _consultationRepo.AddAsync(consultation);
            return _mapper.Map<ConsultationResponseDto>(consultation);
        }

        // ✅ Dapper-based reads
        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByDoctorAsync(int doctorId, int? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10)
            => await _consultationReadRepo.GetConsultationsByDoctorAsync(doctorId, status, fromDate, toDate, page, pageSize);

        public async Task<PagedResult<ConsultationListDto>> GetConsultationsByPatientAsync(int patientId, int? status = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10)
            => await _consultationReadRepo.GetConsultationsByPatientAsync(patientId, status, fromDate, toDate, page, pageSize);

        public async Task<ConsultationDetailsDto?> GetConsultationDetailsAsync(int consultationId, CancellationToken ct = default)
            => await _consultationReadRepo.GetConsultationDetailsAsync(consultationId);

        // Placeholder for future steps (to be implemented)
        public Task<ConsultationResponseDto> UpdateConsultationAsync(int consultationId, ConsultationUpdateDto dto, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<ConsultationResponseDto> FinalizeConsultationAsync(int consultationId, CancellationToken ct = default)
            => throw new NotImplementedException();

        public Task<FileDto> UploadAttachmentAsync(int consultationId, UploadFileDto dto, CancellationToken ct = default)
            => throw new NotImplementedException();
    }
}
