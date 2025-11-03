using Health.Application.DTOs;
using Health.Application.Interfaces;
using Health.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Health.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protect endpoints
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Get paginated appointments for the logged-in doctor.
        /// </summary>
        [Authorize(Roles = "Doctor")]
        [HttpGet("doctor")]
        public async Task<IActionResult> GetDoctorAppointments(
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (userId == 0)
                return Unauthorized(ApiResponse<string>.ErrorResponse("Invalid or missing user token."));

            var (appointments, totalCount) = await _appointmentService.GetDoctorAppointmentsAsync(
                userId, status, page, pageSize);

            var response = new
            {
                TotalCount = totalCount,
                Data = appointments
            };

            return Ok(ApiResponse<object>.SuccessResponse(response, "Appointments fetched successfully."));
        }

        /// <summary>
        /// Get paginated appointments for the logged-in patient.
        /// </summary>
        [Authorize(Roles = "Patient")]
        [HttpGet("patient")]
        public async Task<IActionResult> GetPatientAppointments(
            [FromQuery] int? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            if (userId == 0)
                return Unauthorized(ApiResponse<string>.ErrorResponse("Invalid or missing user token."));

            var (appointments, totalCount) = await _appointmentService.GetPatientAppointmentsAsync(
                userId, status, page, pageSize);

            var response = new
            {
                TotalCount = totalCount,
                Data = appointments
            };

            return Ok(ApiResponse<object>.SuccessResponse(response, "Appointments fetched successfully."));
        }
    }
}
