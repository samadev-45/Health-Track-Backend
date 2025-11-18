using Health.Application.Common;
using Health.Application.DTOs.Common;
using Health.Application.DTOs.MedicalRecord;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Health.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class MedicalRecordsController : ControllerBase
    {
        private readonly IMedicalRecordService _service;

        public MedicalRecordsController(IMedicalRecordService service)
            => _service = service;

        private int CurrentUserId =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);


       
        [Authorize(Roles = "Patient")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponse<MedicalRecordDto>>> Create(
            [FromForm] MedicalRecordUploadRequest model,
            CancellationToken ct)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest(ApiResponse<MedicalRecordDto>.ErrorResponse("File is empty"));

            using var ms = new MemoryStream();
            await model.File.CopyToAsync(ms, ct);

            var dto = new CreateMedicalRecordDto
            {
                RecordTypeId = model.RecordTypeId,
                Title = model.Title,
                Description = model.Description,
                RecordDate = model.RecordDate ?? DateTime.UtcNow,
                DoctorName = model.DoctorName,
                Hospital = model.Hospital,
                FileName = model.File.FileName,
                ContentType = model.File.ContentType,
                FileSize = model.File.Length,
                Base64Data = Convert.ToBase64String(ms.ToArray())
            };

            var created = await _service.CreateAsync(dto, ct);

            return Ok(ApiResponse<MedicalRecordDto>.SuccessResponse(created, "Medical record uploaded successfully"));
        }


        [HttpGet("me")]
        [Authorize(Roles = "Patient")]
        public async Task<ActionResult<ApiResponse<PagedResult<MedicalRecordListDto>>>> GetMyRecords(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? recordTypeId = null,
            [FromQuery] DateTime? from = null,
            [FromQuery] DateTime? to = null)
        {
            var result = await _service.GetMyRecordsAsync(page, pageSize, recordTypeId, from, to);

            return Ok(ApiResponse<PagedResult<MedicalRecordListDto>>.SuccessResponse(result));
        }


        
        
        [HttpGet("{id}")]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<ActionResult<ApiResponse<MedicalRecordDto>>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null)
                return NotFound(ApiResponse<MedicalRecordDto>.ErrorResponse("Record not found"));

            return Ok(ApiResponse<MedicalRecordDto>.SuccessResponse(dto));
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Patient,Admin")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse("Record deleted successfully"));
        }
    }
}
