using AutoMapper;
using Health.Application.DTOs;
using Health.Application.DTOs.Appointment;
using Health.Application.DTOs.Consultation;
using Health.Application.DTOs.File;
using Health.Application.DTOs.MedicalRecord;
using Health.Application.DTOs.Medication;
using Health.Application.DTOs.Prescription;
using Health.Application.DTOs.HealthMetric;
using Health.Application.DTOs.Dashboard;
using Health.Domain.Entities;
using Health.Application.DTOs.Auth;

namespace Health.Application.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ------------------ USER ------------------
            CreateMap<RegisterDto, User>()
                .ForMember(d => d.PasswordHash, o => o.Ignore())
                .ForMember(d => d.RefreshTokens, o => o.Ignore())
                .ForMember(d => d.Caretakers, o => o.Ignore())
                .ForMember(d => d.PatientsUnderCare, o => o.Ignore())
                .ForMember(d => d.MedicalRecords, o => o.Ignore())
                .ForMember(d => d.AppointmentsAsPatient, o => o.Ignore())
                .ForMember(d => d.AppointmentsAsDoctor, o => o.Ignore())
                .ForMember(d => d.Medications, o => o.Ignore())
                .ForMember(d => d.Notifications, o => o.Ignore())
                .ForMember(d => d.ShareableLinks, o => o.Ignore())
                .ForMember(d => d.UploadedFiles, o => o.Ignore())
                .ForMember(d => d.BloodType, o => o.Ignore());

            // ------------------ APPOINTMENTS ------------------
            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<RescheduleAppointmentDto, Appointment>();

            CreateMap<Appointment, AppointmentDto>()
                .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.FullName))
                .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            // ------------------ CONSULTATION ------------------
            CreateMap<Consultation, ConsultationResponseDto>()
                .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.FullName))
                .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<ConsultationCreateDto, Consultation>()
                .ForMember(d => d.PatientId, o => o.Ignore())
                .ForMember(d => d.DoctorId, o => o.Ignore());

          
            CreateMap<FileStorage, FileDownloadDto>()
                .ForMember(d => d.FileBytes, o => o.MapFrom(f => f.FileData));

            CreateMap<FileStorage, FileDto>();

            
            CreateMap<MedicalRecord, MedicalRecordListDto>();

            CreateMap<MedicalRecord, MedicalRecordDto>()
                .ForMember(d => d.FileName, o => o.MapFrom(s => s.File.FileName))
                .ForMember(d => d.ContentType, o => o.MapFrom(s => s.File.ContentType))
                .ForMember(d => d.FileSize, o => o.MapFrom(s => s.File.FileSize));

         
            CreateMap<Medication, MedicationDto>()
                .ForMember(d => d.UnitName,
                    o => o.MapFrom(s => s.Unit != null ? s.Unit.UnitName : null));

            CreateMap<Medication, MedicationListDto>();
            CreateMap<MedicationCreateDto, Medication>();

            CreateMap<Medication, MedicationScheduleItemDto>()
                .ForMember(d => d.MedicationName, o => o.MapFrom(s => s.Name));

          
            CreateMap<MetricType, HealthMetricDetailsDto>();
            CreateMap<MetricType, HealthMetricListDto>();
            CreateMap<MetricType, AbnormalMetricDto>();

            CreateMap<HealthMetric, HealthMetricListDto>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.MetricType.DisplayName))
                .ForMember(d => d.Unit, o => o.MapFrom(s => s.MetricType.Unit))
                .ForMember(d => d.MetricCode, o => o.MapFrom(s => s.MetricType.MetricCode));

            CreateMap<HealthMetric, HealthMetricDetailsDto>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.MetricType.DisplayName))
                .ForMember(d => d.Unit, o => o.MapFrom(s => s.MetricType.Unit))
                .ForMember(d => d.MetricCode, o => o.MapFrom(s => s.MetricType.MetricCode));


            CreateMap<Appointment, NextAppointmentDto>()
                .ForMember(d => d.DoctorName, o => o.MapFrom(s => s.Doctor.FullName));

           

            CreateMap<Notification, NotificationDto>();

            // in MappingProfile
            CreateMap<Appointment, DoctorAppointmentItemDto>()
                .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName));

            CreateMap<Consultation, DoctorConsultationItemDto>()
                .ForMember(d => d.PatientName, o => o.MapFrom(s => s.Patient.FullName));

            CreateMap<User, DoctorPatientItemDto>()
                .ForMember(d => d.Age, o => o.MapFrom(u => u.DateOfBirth.HasValue ? (int?)DateTime.UtcNow.Year - u.DateOfBirth.Value.Year ?? 0 : 0))
                .ForMember(d => d.Gender, o => o.MapFrom(u => u.Gender.ToString()));

            
            CreateMap<PrescriptionCreateDto, Prescription>();
            CreateMap<Prescription, PrescriptionDto>();
            CreateMap<Prescription, PrescriptionDto>();
            CreateMap<PrescriptionItemCreateDto, PrescriptionItem>();
            CreateMap<PrescriptionItem, PrescriptionItemDto>();

            CreateMap<PrescriptionItemUpdateDto, PrescriptionItem>();

            // ------------------ PATIENT PROFILE ------------------
            CreateMap<User, PatientProfileDto>()
     .ForMember(d => d.BloodTypeName,
         o => o.MapFrom(s => s.BloodType != null ? s.BloodType.Name : null))
     .ForMember(d => d.Gender,
         o => o.MapFrom(s => s.Gender))
     .ForMember(d => d.BloodTypeId,
         o => o.MapFrom(s => s.BloodTypeId));



            CreateMap<UpdatePatientProfileDto, User>()
    .ForMember(d => d.Gender, o => o.Ignore())          // we parse gender manually
    .ForMember(d => d.BloodType, o => o.Ignore())       // skip navigation mapping
    .ForMember(d => d.BloodTypeId, o => o.MapFrom(s => s.BloodTypeId));
            // navigation skip

        }
    }
}
