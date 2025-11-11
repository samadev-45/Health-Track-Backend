using AutoMapper;
using Health.Application.DTOs;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Application.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IConsultationRepository _consultationRepo;
        private readonly IAppointmentWriteRepository _appointmentRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public ConsultationService(
            IConsultationRepository consultationRepo,
            IAppointmentWriteRepository appointmentRepo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _consultationRepo = consultationRepo;
            _appointmentRepo = appointmentRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<ConsultationResponseDto> CreateConsultationAsync(
            int appointmentId,
            ConsultationCreateDto dto,
            CancellationToken ct = default)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(appointmentId, ct)
                ?? throw new InvalidOperationException("Appointment not found.");

            if (appointment.Status != Domain.Enums.AppointmentStatus.Completed)
                throw new InvalidOperationException("Consultation can only be created after appointment completion.");

            var patient = await _userRepo.GetByIdAsync(appointment.PatientId, ct)
                ?? throw new InvalidOperationException("Patient not found.");

            var consultation = new Consultation
            {
                AppointmentId = appointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                DoctorNotes = dto.DoctorNotes,
                HealthValuesJson = dto.HealthValuesJson ?? "{}"
            };

            await _consultationRepo.AddAsync(consultation);
            return _mapper.Map<ConsultationResponseDto>(consultation);
        }

        public async Task<IEnumerable<ConsultationResponseDto>> GetConsultationsByUserAsync(
            int userId,
            CancellationToken ct = default)
        {
            var consultations = await _consultationRepo.GetByUserIdAsync(userId, ct);
            return _mapper.Map<IEnumerable<ConsultationResponseDto>>(consultations);
        }
    }
}
