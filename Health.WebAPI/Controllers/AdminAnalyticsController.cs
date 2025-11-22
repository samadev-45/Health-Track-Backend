using Health.Application.Common;
using Health.Application.DTOs.Admin;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Health.WebAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/analytics")]
    [Authorize(Roles = "Admin")]
    public class AdminAnalyticsController : ControllerBase
    {
        private readonly IAdminAnalyticsService _service;

        public AdminAnalyticsController(IAdminAnalyticsService service)
        {
            _service = service;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponse<AdminSummaryDto>>> GetSummary()
        {
            var result = await _service.GetSummaryAsync();
            return Ok(ApiResponse<AdminSummaryDto>.SuccessResponse(result));
        }

        [HttpGet("appointments-chart")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ChartPointDto>>>> GetAppointmentsChart(
            [FromQuery] int days = 7)
        {
            var result = await _service.GetAppointmentsChartAsync(days);
            return Ok(ApiResponse<IEnumerable<ChartPointDto>>.SuccessResponse(result));
        }

        [HttpGet("abnormal-metrics")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AbnormalMetricStatDto>>>> GetAbnormalMetrics()
        {
            var result = await _service.GetAbnormalMetricsAsync();
            return Ok(ApiResponse<IEnumerable<AbnormalMetricStatDto>>.SuccessResponse(result));
        }

        [HttpGet("prescriptions")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PrescriptionStatDto>>>> GetPrescriptionStats(
            [FromQuery] int months = 6)
        {
            var result = await _service.GetPrescriptionStatsAsync(months);
            return Ok(ApiResponse<IEnumerable<PrescriptionStatDto>>.SuccessResponse(result));
        }
    }
}
