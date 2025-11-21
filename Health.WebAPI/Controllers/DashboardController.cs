using Health.Application.Common;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.Dashboard;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Health.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        
        [HttpGet("patient")]
        [Authorize(Roles = "Patient,Doctor")] 
        public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetPatientDashboard(
            [FromQuery] DateTime? date = null,
            [FromQuery] int upcomingDays = 7,
            CancellationToken ct = default)
        {
            var result = await _service.GetPatientDashboardAsync(date, upcomingDays, ct);

            return Ok(ApiResponse<DashboardSummaryDto>.SuccessResponse(result));
        }
    }
}
