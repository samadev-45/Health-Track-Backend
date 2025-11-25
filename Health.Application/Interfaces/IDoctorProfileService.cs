using Health.Application.DTOs.Doctor;

namespace Health.Application.Interfaces
{
    public interface IDoctorProfileService
    {
        Task<UpdateDoctorProfileDto> UpdateProfileAsync(int doctorId, UpdateDoctorProfileDto dto);

        Task<UpdateDoctorProfileDto?> GetProfileAsync(int doctorId);
    }
}
