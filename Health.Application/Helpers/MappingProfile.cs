using AutoMapper;
using Health.Application.DTOs;
using Health.Domain.Entities;

namespace Health.Application.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map RegisterDto -> User
            CreateMap<RegisterDto, User>()
                // Ignore password fields because hashing is done in service
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())

                // Ignore navigation collections so AutoMapper doesn't try to populate them
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.Caretakers, opt => opt.Ignore())
                .ForMember(dest => dest.PatientsUnderCare, opt => opt.Ignore())
                .ForMember(dest => dest.MedicalRecords, opt => opt.Ignore())
                .ForMember(dest => dest.AppointmentsAsPatient, opt => opt.Ignore())
                .ForMember(dest => dest.AppointmentsAsDoctor, opt => opt.Ignore())
                .ForMember(dest => dest.Medications, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore())
                .ForMember(dest => dest.ShareableLinks, opt => opt.Ignore())
                .ForMember(dest => dest.UploadedFiles, opt => opt.Ignore());
        }
    }
}
