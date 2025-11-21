using Health.Application.Common;
using Health.Application.DTOs.Dashboard;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Health.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Doctor")] // doctor-only
    public class DoctorDashboardController : ControllerBase
    {
        private readonly IDoctorDashboardService _service;

        public DoctorDashboardController(IDoctorDashboardService service) => _service = service;

        [HttpGet("overview")]
        public async Task<ActionResult<ApiResponse<DoctorDashboardDto>>> GetOverview(
            [FromQuery] DateTime? date = null,
            [FromQuery] int upcomingDays = 7,
            [FromQuery] int recentCount = 10,
            CancellationToken ct = default)
        {
            var dto = await _service.GetDoctorDashboardAsync(date, upcomingDays, recentCount, ct);
            return Ok(ApiResponse<DoctorDashboardDto>.SuccessResponse(dto));
        }
    }
}
