using Health.Application.Common;
using Health.Application.DTOs;
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

        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("id")
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub");

            if (claim == null) return 0;
            return int.TryParse(claim.Value, out var id) ? id : 0;
        }

        // ----------------------------------------------------------
        // DOCTOR APPOINTMENTS
        // ----------------------------------------------------------
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

            var result = new { TotalCount = total, Records = appointments };

            return Ok(ApiResponse<object>.SuccessResponse(result, "Doctor appointments fetched successfully"));
        }

        // ----------------------------------------------------------
        // PATIENT APPOINTMENTS
        // ----------------------------------------------------------
        [HttpGet("patient")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetPatientAppointments(
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            int patientId = GetUserId();

            var (appointments, total) = await _appointmentService
                .GetPatientAppointmentsAsync(patientId, status, page, pageSize, ct);

            var result = new { TotalCount = total, Records = appointments };

            return Ok(ApiResponse<object>.SuccessResponse(result, "Patient appointments fetched successfully"));
        }

        // ----------------------------------------------------------
        // CREATE (PATIENT)
        // ----------------------------------------------------------
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

        // ----------------------------------------------------------
        // RESCHEDULE (PATIENT)
        // ----------------------------------------------------------
        [HttpPost("{appointmentId}/reschedule")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> RescheduleAppointment(
            int appointmentId,
            [FromBody] RescheduleAppointmentDto dto,
            CancellationToken ct = default)
        {
            int patientId = GetUserId();
            if (patientId <= 0)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or missing user id in token.", 401));

            if (dto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid reschedule data."));

            try
            {
                var result = await _appointmentService.RescheduleAppointmentAsync(appointmentId, patientId, dto, ct);
                return Ok(ApiResponse<object>.SuccessResponse(result, "Appointment rescheduled successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal Server Error", ex.Message, 500));
            }
        }

        // ----------------------------------------------------------
        // CANCEL (PATIENT OR ADMIN)
        // ----------------------------------------------------------
        [HttpPost("{appointmentId}/cancel")]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<IActionResult> CancelAppointment(
            int appointmentId,
            [FromBody] CancelAppointmentDto dto,
            CancellationToken ct = default)
        {
            int userId = GetUserId();
            if (userId <= 0)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or missing user id in token.", 401));

            try
            {
                var result = await _appointmentService.CancelAppointmentAsync(appointmentId, userId, dto, ct);
                return Ok(ApiResponse<object>.SuccessResponse(result, "Appointment cancelled successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal Server Error", ex.Message, 500));
            }
        }

        // ----------------------------------------------------------
        // REASSIGN (ADMIN)
        // ----------------------------------------------------------
        [HttpPost("{appointmentId}/reassign")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReassignAppointment(
            int appointmentId,
            [FromBody] ReassignAppointmentDto dto,
            CancellationToken ct = default)
        {
            int adminId = GetUserId();
            if (adminId <= 0)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or missing user id in token.", 401));

            if (dto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid reassignment data."));

            try
            {
                var result = await _appointmentService.ReassignAppointmentAsync(appointmentId, adminId, dto, ct);
                return Ok(ApiResponse<object>.SuccessResponse(result, "Appointment reassigned successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal Server Error", ex.Message, 500));
            }
        }

        
        // COMPLETE (DOCTOR)
        
        [HttpPost("{appointmentId}/complete")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CompleteAppointment(
            int appointmentId,
            [FromBody] CompleteAppointmentDto dto,
            CancellationToken ct = default)
        {
            int doctorId = GetUserId();
            if (doctorId <= 0)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or missing user id in token.", 401));

            try
            {
                var result = await _appointmentService.CompleteAppointmentAsync(appointmentId, doctorId, dto, ct);
                return Ok(ApiResponse<object>.SuccessResponse(result, "Appointment marked as completed"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal Server Error", ex.Message, 500));
            }
        }
        [HttpPost("{appointmentId}/respond")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> RespondToAppointment(
            int appointmentId,
            [FromBody] RespondToAppointmentDto dto,
            CancellationToken ct = default)
        {
            int doctorId = GetUserId();
            if (doctorId <= 0)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or missing user id in token.", 401));

            try
            {
                var result = await _appointmentService.RespondToAppointmentAsync(appointmentId, doctorId, dto, ct);
                string message = dto.IsAccepted ? "Appointment accepted successfully" : "Appointment rejected successfully";
                return Ok(ApiResponse<object>.SuccessResponse(result, message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal Server Error", ex.Message, 500));
            }
        }
        [HttpGet("{appointmentId}/history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAppointmentHistory(int appointmentId, CancellationToken ct = default)
        {
            var history = await _appointmentService.GetAppointmentHistoryAsync(appointmentId, ct);
            return Ok(ApiResponse<object>.SuccessResponse(history, "Appointment history fetched successfully"));
        }

    }
}
