using Health.Application.DTOs;
using Health.Application.DTOs.Common;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Health.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        // Helper: get current user ID from JWT
        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        private string CurrentRole =>
            User.FindFirstValue(ClaimTypes.Role) ?? "";

        // ---------------------------------------------------------------------
        // 1. CREATE CONSULTATION (Doctor Only)
        // ---------------------------------------------------------------------
        [HttpPost("{appointmentId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateConsultation(
    int appointmentId,
    [FromBody] ConsultationCreateDto dto,
    CancellationToken ct)
        {
            dto.AppointmentId = appointmentId;

            var result = await _consultationService.CreateConsultationAsync(
                appointmentId, dto, ct);

            return Ok(result);
        }


        // ---------------------------------------------------------------------
        // 2. UPDATE CONSULTATION (Doctor Only, Ownership inside service)
        // ---------------------------------------------------------------------
        [HttpPut("{consultationId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateConsultation(
            int consultationId,
            [FromBody] ConsultationUpdateDto dto,
            CancellationToken ct)
        {
            var result = await _consultationService.UpdateConsultationAsync(
                consultationId, dto, ct);

            return Ok(result);
        }

        // ---------------------------------------------------------------------
        // 3. FINALIZE CONSULTATION (Doctor Only)
        // ---------------------------------------------------------------------
        [HttpPost("{consultationId}/finalize")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> FinalizeConsultation(
            int consultationId,
            CancellationToken ct)
        {
            var result = await _consultationService.FinalizeConsultationAsync(
                consultationId, ct);

            return Ok(result);
        }

        // ---------------------------------------------------------------------
        // 4. UPLOAD ATTACHMENT (Doctor or Patient)
        // ---------------------------------------------------------------------
        [HttpPost("{consultationId}/attachments")]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> UploadAttachment(
            int consultationId,
            IFormFile file,
            CancellationToken ct)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);

            var dto = new UploadFileDto
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                Base64Data = Convert.ToBase64String(ms.ToArray()),
                UploadedByUserId = CurrentUserId
            };

            var result = await _consultationService.UploadAttachmentAsync(
                consultationId, dto, ct);

            return Ok(result);
        }

        // ---------------------------------------------------------------------
        // 5. GET DOCTOR CONSULTATIONS (Paged)
        // Doctor ALWAYS sees only their own consultations
        // ---------------------------------------------------------------------
        [HttpGet("doctor")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetConsultationsByDoctor(
            int? status,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 10)
        {
            int doctorId = CurrentUserId; // secure

            var result = await _consultationService.GetConsultationsByDoctorAsync(
                doctorId, status, fromDate, toDate, page, pageSize);

            return Ok(result);
        }

        // ---------------------------------------------------------------------
        // 6. GET PATIENT CONSULTATIONS (Paged)
        // Patient ALWAYS sees only their own consultations
        // ---------------------------------------------------------------------
        [HttpGet("patient")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetConsultationsByPatient(
            int? status,
            DateTime? fromDate,
            DateTime? toDate,
            int page = 1,
            int pageSize = 10)
        {
            int patientId = CurrentUserId; // secure

            var result = await _consultationService.GetConsultationsByPatientAsync(
                patientId, status, fromDate, toDate, page, pageSize);

            return Ok(result);
        }

        // ---------------------------------------------------------------------
        // 7. GET CONSULTATION DETAILS (Doctor/Patient Ownership Enforced)
        // ---------------------------------------------------------------------
        [HttpGet("{consultationId}")]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> GetConsultationDetails(
            int consultationId,
            CancellationToken ct)
        {
            var details = await _consultationService.GetConsultationDetailsAsync(
                consultationId, ct);

            if (details == null)
                return NotFound("Consultation not found.");

            return Ok(details);
        }

        // ------------------------------------------------------------
        // DOWNLOAD FILE (Doctor or Patient)
        // ------------------------------------------------------------
        [HttpGet("files/{fileId}")]
        [Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> DownloadFile(int fileId, CancellationToken ct)
        {
            var file = await _consultationService.DownloadFileAsync(fileId, ct);

            return File(file.FileBytes, file.ContentType, file.FileName);
        }

    }
}
