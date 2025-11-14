using AutoMapper;
using Health.Application.DTOs;
using Health.Application.DTOs.Appointments;
using Health.Domain.Entities;

namespace Health.Application.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 🧩 Map RegisterDto → User
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

            // 🧩 Map Appointment-related DTOs
            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<RescheduleAppointmentDto, Appointment>();

            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Consultation, ConsultationResponseDto>()
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn));

            // Map from DTO → Entity
            CreateMap<ConsultationCreateDto, Consultation>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.AppointmentId))
                .ForMember(dest => dest.DoctorNotes, opt => opt.MapFrom(src => src.DoctorNotes))
                .ForMember(dest => dest.HealthValues, opt => opt.MapFrom(src => src.HealthValues))

                // PatientId will be assigned inside the service (from Appointment)
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());


        }
    }
}
