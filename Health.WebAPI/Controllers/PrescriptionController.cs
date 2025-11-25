using Health.Application.Common;
using Health.Application.DTOs;
using Health.Application.DTOs.Prescription;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/consultations/{consultationId}/[controller]")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly IConsultationService _service;
    public PrescriptionController(IConsultationService service) { _service = service; }

    // Create or get prescription for consultation
    [HttpPost]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<IActionResult> CreateOrGet(int consultationId, [FromBody] PrescriptionCreateDto dto, CancellationToken ct)
    {
        var pres = await _service.CreateOrGetPrescriptionAsync(consultationId, dto, ct);
        return Ok(ApiResponse<PrescriptionDto>.SuccessResponse(pres, "Prescription created or fetched successfully"));
    }

    // Get prescription by consultation
    [HttpGet]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<IActionResult> Get(int consultationId, CancellationToken ct)
    {
        var pres = await _service.GetPrescriptionByConsultationAsync(consultationId, ct);
        if (pres == null)
            return NotFound(ApiResponse<PrescriptionDto>.ErrorResponse("Prescription not found.", 404));
        return Ok(ApiResponse<PrescriptionDto>.SuccessResponse(pres, "Prescription fetched successfully"));
    }

    // Add item
    [HttpPost("items")]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<IActionResult> AddItem(int consultationId, [FromBody] PrescriptionItemCreateDto dto, CancellationToken ct)
    {
        var pres = await _service.GetPrescriptionByConsultationAsync(consultationId, ct);
        if (pres == null)
            return NotFound(ApiResponse<PrescriptionItemDto>.ErrorResponse("Prescription not found. Create prescription first.", 404));

        var item = await _service.AddPrescriptionItemAsync(pres.PrescriptionId, dto, ct);
        return Ok(ApiResponse<PrescriptionItemDto>.SuccessResponse(item, "Item added successfully"));
    }

    // Update item
    [HttpPut("items/{itemId}")]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<IActionResult> UpdateItem(int consultationId, int itemId, [FromBody] PrescriptionItemUpdateDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdatePrescriptionItemAsync(itemId, dto, ct);
        return Ok(ApiResponse<PrescriptionItemDto>.SuccessResponse(updated, "Item updated successfully"));
    }

    // Delete item
    [HttpDelete("items/{itemId}")]
    [Authorize(Roles = "Doctor,Patient")]
    public async Task<IActionResult> DeleteItem(int consultationId, int itemId, CancellationToken ct)
    {
        await _service.DeletePrescriptionItemAsync(itemId, ct);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Item deleted successfully"));
    }
}
