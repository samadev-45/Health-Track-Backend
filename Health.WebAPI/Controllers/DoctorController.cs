using Health.Application.Common;
using Health.Application.DTOs.Doctor;
using Health.Application.Interfaces;
using Health.Application.Interfaces.EFCore;
using Health.Application.Services;
using Health.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Health.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorProfileService _profileService;
        private readonly IUserRepository _userRepo;
        private readonly IDoctorProfileRepository _profileRepo;
        private readonly IAppointmentService _appointmentService;
        public DoctorController(
            IDoctorProfileService profileService,
            IUserRepository userRepo,
            IDoctorProfileRepository profileRepo,
            IAppointmentService appointmentService)
        {
            _profileService = profileService;
            _userRepo = userRepo;
            _profileRepo = profileRepo;
            _appointmentService = appointmentService;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                      ?? User.FindFirst("id")
                      ?? User.FindFirst("sub");

            if (claim == null) return 0;
            return int.TryParse(claim.Value, out var id) ? id : 0;
        }

        // ------------------------------------------------------
        //  GET: Doctor Profile (Doctor Only)
        // ------------------------------------------------------
        [HttpGet("me")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyProfile()
        {
            var doctorId = GetUserId();
            if (doctorId <= 0)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid token", 401));

            var profile = await _profileService.GetProfileAsync(doctorId);
            if (profile == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Profile not found", 404));

            return Ok(ApiResponse<UpdateDoctorProfileDto>.SuccessResponse(profile, "Profile fetched successfully"));
        }

        //public IDoctorProfileService Get_profileService()
        //{
        //    return _profileService;
        //}

        // ------------------------------------------------------
        //  PUT: Update Doctor Profile (Doctor Only)
        // ------------------------------------------------------
        [HttpPut("me")]

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateProfile(
    [FromBody] UpdateDoctorProfileDto dto,
    CancellationToken ct)
        {
            int doctorId = GetUserId();

            var updated = await _profileService.UpdateProfileAsync(doctorId, dto);

            return Ok(ApiResponse<UpdateDoctorProfileDto>.SuccessResponse(
                updated,
                "Profile updated successfully"
            ));
        }


        // ------------------------------------------------------
        //  GET: All Doctors (With Profile + General Info) — Admin only
        // ------------------------------------------------------
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDoctors(CancellationToken ct)
        {
            var users = (await _userRepo.GetDoctorsAsync(ct)).ToList();

            var result = new List<object>();

            foreach (var u in users)
            {
                var profile = await _profileRepo.GetByUserIdAsync(u.UserId, ct);

                result.Add(new
                {
                    id = u.UserId,
                    fullName = u.FullName,
                    email = u.Email,
                    specialtyId = u.SpecialtyId,
                    licenseNumber = u.LicenseNumber,
                    isVerified = u.IsVerified,
                    hospital = profile?.Hospital,
                    location = profile?.Location,
                    clinicName = profile?.ClinicName,
                    about = profile?.About,
                    experienceYears = profile?.ExperienceYears,
                    consultationFee = profile?.ConsultationFee,
                    availableFrom = profile?.AvailableFrom?.ToString(@"hh\:mm"),
                    availableTo = profile?.AvailableTo?.ToString(@"hh\:mm"),
                    availableDays = profile?.AvailableDays
                });
            }

            return Ok(ApiResponse<object>.SuccessResponse(result, "Doctor list fetched successfully"));
        }

        [HttpGet("list")]
        [AllowAnonymous]   // or [Authorize(Roles="Patient,Admin")] your choice
        public async Task<IActionResult> GetDoctorsList(CancellationToken ct)
        {
            var users = (await _userRepo.GetDoctorsAsync(ct)).ToList();
            var result = new List<object>();

            foreach (var u in users)
            {
                var profile = await _profileRepo.GetByUserIdAsync(u.UserId, ct);

                result.Add(new
                {
                    id = u.UserId,
                    fullName = u.FullName,
                    isVerified = u.IsVerified,
                    hospital = profile?.Hospital,
                    location = profile?.Location
                });
            }

            return Ok(result);
        }
        [HttpGet("availability/{doctorId}")]
        [Authorize]
        public async Task<IActionResult> GetAvailability(
    int doctorId,
    [FromQuery] DateTime date
    )
        {
            var result = await _appointmentService.GetAvailableSlotsAsync(doctorId, date);

            return Ok(ApiResponse<object>.SuccessResponse(result, "Availability loaded"));
        }

    
    [HttpPut("availability/days")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateAvailableDays([FromBody] UpdateAvailableDaysDto dto)
        {
            int doctorId = GetUserId();

            if (dto.Days == null || dto.Days.Count == 0)
                return BadRequest(ApiResponse<object>.ErrorResponse("Days list cannot be empty"));

            // Convert List<string> → AvailableDays enum flags
            AvailableDays flags = AvailableDays.None;

            foreach (var day in dto.Days)
            {
                if (!Enum.TryParse<AvailableDays>(day, true, out var parsed))
                    return BadRequest(ApiResponse<object>.ErrorResponse($"Invalid day: {day}"));

                flags |= parsed;
            }

            var profile = await _profileRepo.GetByUserIdAsync(doctorId);
            if (profile == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Doctor profile not found"));

            profile.AvailableDays = flags;
            await _profileRepo.UpdateAsync(profile);

            return Ok(ApiResponse<object>.SuccessResponse(null, "Available days updated"));
        }

    }
}
