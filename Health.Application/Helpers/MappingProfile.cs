using AutoMapper;
using Health.Application.DTOs;
using Health.Application.DTOs.Appointment;
using Health.Application.DTOs.Consultation;
using Health.Application.DTOs.Prescription;
using Health.Domain.Entities;

namespace Health.Application.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ----------------------
            // USER → ENTITY MAPPING
            // ----------------------
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.Caretakers, opt => opt.Ignore())
                .ForMember(dest => dest.PatientsUnderCare, opt => opt.Ignore())
                .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
                .ForMember(dest => dest.AppointmentsAsPatient, opt => opt.Ignore())
                .ForMember(dest => dest.AppointmentsAsDoctor, opt => opt.Ignore())
                .ForMember(dest => dest.Medications, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForMember(dest => dest.ShareableLinks, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedFiles, opt => opt.Ignore())
                .ForMember(dest => dest.BloodType, opt => opt.Ignore());

            // ----------------------
            // APPOINTMENT MAPPING
            // ----------------------
            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<RescheduleAppointmentDto, Appointment>();

            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // ----------------------
            // CONSULTATION → RESPONSE DTO
            // ----------------------
            CreateMap<Consultation, ConsultationResponseDto>()
                .ForMember(dest => dest.DoctorName,
                    opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : null))
                .ForMember(dest => dest.PatientName,
                    opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : null))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.HealthValues,
                    opt => opt.MapFrom(src => src.HealthValues))
                .ForMember(dest => dest.TrendSummary,
                    opt => opt.MapFrom(src => src.TrendSummary))
                .ForMember(dest => dest.CreatedOn,
                    opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.ModifiedOn,
                    opt => opt.MapFrom(src => src.ModifiedOn));

            // ----------------------
            // CREATE DTO → ENTITY MAPPING
            // ----------------------
            CreateMap<ConsultationCreateDto, Consultation>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.AppointmentId))
                .ForMember(dest => dest.ChiefComplaint, opt => opt.MapFrom(src => src.ChiefComplaint))
                .ForMember(dest => dest.Diagnosis, opt => opt.MapFrom(src => src.Diagnosis))
                .ForMember(dest => dest.Advice, opt => opt.MapFrom(src => src.Advice))
                .ForMember(dest => dest.DoctorNotes, opt => opt.MapFrom(src => src.DoctorNotes))
                .ForMember(dest => dest.HealthValues, opt => opt.MapFrom(src => src.HealthValues))
                .ForMember(dest => dest.FollowUpDate, opt => opt.MapFrom(src => src.FollowUpDate))

                // Assigned inside service
                .ForMember(dest => dest.PatientId, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore());

            CreateMap<Prescription, PrescriptionDto>()
    .ForMember(d => d.Items, opt => opt.MapFrom(src => src.Items ?? new List<PrescriptionItem>()));

            CreateMap<PrescriptionCreateDto, Prescription>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedByUserId, opt => opt.Ignore());

            CreateMap<PrescriptionItemCreateDto, PrescriptionItem>();
            CreateMap<PrescriptionItemUpdateDto, PrescriptionItem>();
            CreateMap<PrescriptionItem, PrescriptionItemDto>();

        }
    }
}
