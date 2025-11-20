using Health.Application.Common;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.HealthMetric;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Health.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient,Doctor,Admin")]
    public class HealthMetricsController : ControllerBase
    {
        private readonly IHealthMetricService _service;

        public HealthMetricsController(IHealthMetricService service)
        {
            _service = service;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

       
        [HttpPost]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<ApiResponse<HealthMetricDetailsDto>>> Create(
            [FromBody] CreateHealthMetricDto dto,
            CancellationToken ct)
        {
            var result = await _service.CreateAsync(dto, ct);
            return Ok(ApiResponse<HealthMetricDetailsDto>.SuccessResponse(result));
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<ApiResponse<HealthMetricDetailsDto>>> Update(
            int id,
            [FromBody] HealthMetricUpdateDto dto,
            CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, dto, ct);
            return Ok(ApiResponse<HealthMetricDetailsDto>.SuccessResponse(result));
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return Ok(ApiResponse<string>.SuccessResponse("Deleted successfully"));
        }

        
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<PagedResult<HealthMetricListDto>>>> GetMyMetrics(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? metricTypeId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var result = await _service.GetForUserAsync(
                page, pageSize, metricTypeId, from, to);

            return Ok(ApiResponse<PagedResult<HealthMetricListDto>>.SuccessResponse(result));
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<HealthMetricDetailsDto>>> GetById(
            int id,
            CancellationToken ct)
        {
            var result = await _service.GetByIdAsync(id, ct);
            return Ok(ApiResponse<HealthMetricDetailsDto>.SuccessResponse(result));
        }

       
        [HttpGet("abnormal/today")]
        public async Task<ActionResult<ApiResponse<IEnumerable<HealthMetricListDto>>>> GetTodayAbnormal(
            CancellationToken ct)
        {
            var result = await _service.GetTodayAbnormalAsync(ct);
            return Ok(ApiResponse<IEnumerable<HealthMetricListDto>>.SuccessResponse(result));
        }


        [HttpGet("trend")]
        public async Task<ActionResult<ApiResponse<MetricTrendDto>>> GetTrend(
    [FromQuery] int metricTypeId,
    [FromQuery] int days = 30,
    CancellationToken ct = default)
        {
            var result = await _service.GetTrendAsync(metricTypeId, days, ct);
            return Ok(ApiResponse<MetricTrendDto>.SuccessResponse(result));
        }

    }
}
