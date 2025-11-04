using Health.Application.Common;
using Health.Application.DTOs.Appointments;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Health.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentController(
            IAppointmentService appointmentService,
            IHttpContextAccessor httpContextAccessor)
        {
            _appointmentService = appointmentService;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ Extract UserId from JWT claims

        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("id")
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub");

            if (claim == null) return 0;

            return int.TryParse(claim.Value, out var id) ? id : 0;
        }

        /// <summary>
        /// Get paginated appointments for the logged-in doctor
        /// </summary>
        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetDoctorAppointments(
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            int doctorId = GetUserId();

            var (appointments, total) = await _appointmentService
                .GetDoctorAppointmentsAsync(doctorId, status, page, pageSize, ct);

            var result = new
            {
                TotalCount = total,
                Records = appointments
            };

            return Ok(ApiResponse<object>.SuccessResponse(result, "Doctor appointments fetched successfully"));
        }

        /// <summary>
        /// Get paginated appointments for the logged-in patient
        /// </summary>
        [HttpGet("patient")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetPatientAppointments(
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            int patientId = GetUserId();

            var (appointments, total) = await _appointmentService
                .GetPatientAppointmentsAsync(patientId, status, page, pageSize, ct);

            var result = new
            {
                TotalCount = total,
                Records = appointments
            };

            return Ok(ApiResponse<object>.SuccessResponse(result, "Patient appointments fetched successfully"));
        }

        /// <summary>
        /// Create a new appointment (patient books a doctor)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> CreateAppointment(
            [FromBody] CreateAppointmentDto dto,
            CancellationToken ct = default)
        {
            int patientId = GetUserId();
            if (patientId <= 0)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or missing user id in token.", 401));

            try
            {
                var appointmentId = await _appointmentService.CreateAppointmentAsync(patientId, dto, ct);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { AppointmentId = appointmentId },
                    "Appointment created successfully",
                    201
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal Server Error", ex.Message, 500));
            }
        }
    }
}
