using Health.Application.Common;
using Health.Application.DTOs;
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
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConsultationController(
            IConsultationService consultationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _consultationService = consultationService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Extract userId (from JWT)
        private int GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("id")
                        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub");

            return claim != null && int.TryParse(claim.Value, out var id) ? id : 0;
        }

        // ----------------------------------------------------------
        // 1️. CREATE CONSULTATION (Doctor only)
        // ----------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateConsultation(
            [FromBody] ConsultationCreateDto dto,
            CancellationToken ct = default)
        {
            int doctorId = GetUserId();
            if (doctorId <= 0)
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or missing user id in token.", 401));

            if (dto == null)
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid consultation data."));

            try
            {
                var consultation = await _consultationService.CreateConsultationAsync(dto.AppointmentId, dto, ct);
                return Ok(ApiResponse<object>.SuccessResponse(consultation, "Consultation created successfully."));
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
        // 2️⃣ GET CONSULTATIONS BY USER (Doctor or Patient)
        // ----------------------------------------------------------
        [HttpGet("{userId}")]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> GetConsultationsByUser(
            int userId,
            CancellationToken ct = default)
        {
            try
            {
                var consultations = await _consultationService.GetConsultationsByUserAsync(userId, ct);
                return Ok(ApiResponse<object>.SuccessResponse(consultations, "Consultations fetched successfully."));
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
