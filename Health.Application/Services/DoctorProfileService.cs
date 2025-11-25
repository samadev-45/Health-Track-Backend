using Health.Application.DTOs.Doctor;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Domain.Entities;
using Health.Domain.Enums;

namespace Health.Application.Services
{
    public class DoctorProfileService : IDoctorProfileService
    {
        private readonly IDoctorProfileRepository _profileRepo;
        private readonly IUserRepository _userRepo;

        public DoctorProfileService(
            IDoctorProfileRepository profileRepo,
            IUserRepository userRepo)
        {
            _profileRepo = profileRepo;
            _userRepo = userRepo;
        }

        public async Task<UpdateDoctorProfileDto> UpdateProfileAsync(int doctorId, UpdateDoctorProfileDto dto)


        {
            var user = await _userRepo.GetByIdAsync(doctorId);
            if (user == null || user.Role != RoleType.Doctor)
                throw new UnauthorizedAccessException("Invalid doctor account.");

            var profile = await _profileRepo.GetByUserIdAsync(doctorId);
            if (profile == null)
                throw new InvalidOperationException("Doctor profile not found.");

            profile.Hospital = dto.Hospital;
            profile.Location = dto.Location;
            profile.ClinicName = dto.ClinicName;
            profile.About = dto.About;
            profile.ExperienceYears = dto.ExperienceYears;
            profile.ConsultationFee = dto.ConsultationFee;
            profile.AvailableFrom = dto.AvailableFrom;
            profile.AvailableTo = dto.AvailableTo;
            profile.AvailableDays = dto.AvailableDays;   // ⭐ enum mapping is direct

            await _profileRepo.UpdateAsync(profile);
            return dto;
        }

        public async Task<UpdateDoctorProfileDto?> GetProfileAsync(int doctorId)
        {
            var profile = await _profileRepo.GetByUserIdAsync(doctorId);
            if (profile == null) return null;

            return new UpdateDoctorProfileDto
            {
                Hospital = profile.Hospital,
                Location = profile.Location,
                ClinicName = profile.ClinicName,
                About = profile.About,
                ExperienceYears = profile.ExperienceYears,
                ConsultationFee = profile.ConsultationFee,
                AvailableFrom = profile.AvailableFrom,
                AvailableTo = profile.AvailableTo,
                AvailableDays = profile.AvailableDays   // ⭐ return enum value
            };
        }
    }
}
