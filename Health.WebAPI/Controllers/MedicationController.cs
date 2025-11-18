using Health.Application.Common;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.Medication;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Health.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient,Doctor,Admin")]
    public class MedicationsController : ControllerBase
    {
        private readonly IMedicationService _service;

        public MedicationsController(IMedicationService service)
        {
            _service = service;
        }

        private int CurrentUserId =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        
        [HttpPost]
        [Authorize(Roles = "Patient,Doctor")] 
        public async Task<ActionResult<ApiResponse<MedicationDto>>> CreateMedication(
            [FromBody] MedicationCreateDto dto,
            CancellationToken ct)
        {
            var created = await _service.AddMedicationAsync(dto, ct);

            return Ok(ApiResponse<MedicationDto>.SuccessResponse(created));
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<ApiResponse<MedicationDto>>> UpdateMedication(
            int id,
            [FromBody] MedicationUpdateDto dto,
            CancellationToken ct)
        {
            var updated = await _service.UpdateMedicationAsync(id, dto, ct);

            return Ok(ApiResponse<MedicationDto>.SuccessResponse(updated));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteMedication(
            int id,
            CancellationToken ct)
        {
            var success = await _service.DeleteMedicationAsync(id, ct);

            if (!success)
                return BadRequest(ApiResponse<string>.ErrorResponse("Unable to delete medication."));

            return Ok(ApiResponse<string>.SuccessResponse("Medication deleted successfully."));
        }

        
        [HttpGet("me")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<ApiResponse<PagedResult<MedicationListDto>>>> GetMyMedications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await _service.GetMyMedicationsAsync(page, pageSize, ct);

            return Ok(ApiResponse<PagedResult<MedicationListDto>>.SuccessResponse(result));
        }

        
        [HttpGet("schedule/today")]
        [Authorize(Roles = "Patient,Doctor")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MedicationScheduleItemDto>>>> GetTodaySchedule(
            CancellationToken ct = default)
        {
            var result = await _service.GetScheduleForTodayAsync(ct);

            return Ok(ApiResponse<IEnumerable<MedicationScheduleItemDto>>.SuccessResponse(result));
        }

       
        [HttpGet("{id}")]
        [Authorize(Roles = "Patient,Doctor,Admin")]
        public async Task<ActionResult<ApiResponse<MedicationDto>>> GetMedicationById(
            int id,
            CancellationToken ct = default)
        {
            var result = await _service.GetByIdAsync(id, ct);

            if (result == null)
                return NotFound(ApiResponse<MedicationDto>.ErrorResponse("Medication not found."));

            return Ok(ApiResponse<MedicationDto>.SuccessResponse(result));
        }
    }
}
