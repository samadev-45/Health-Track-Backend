using Health.Application.DTOs;
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
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> CreateOrGet(int consultationId, [FromBody] PrescriptionCreateDto dto, CancellationToken ct)
    {
        var pres = await _service.CreateOrGetPrescriptionAsync(consultationId, dto, ct);
        return Ok(pres);
    }

    // Get prescription by consultation
    [HttpGet]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> Get(int consultationId, CancellationToken ct)
    {
        var pres = await _service.GetPrescriptionByConsultationAsync(consultationId, ct);
        if (pres == null) return NotFound();
        return Ok(pres);
    }

    // Add item
    [HttpPost("items")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> AddItem(int consultationId, [FromBody] PrescriptionItemCreateDto dto, CancellationToken ct)
    {
        // need to locate prescriptionId for consultation
        var pres = await _service.GetPrescriptionByConsultationAsync(consultationId, ct);
        if (pres == null) return NotFound("Prescription not found. Create prescription first.");

        var item = await _service.AddPrescriptionItemAsync(pres.PrescriptionId, dto, ct);
        return Ok(item);
    }

    // Update item
    [HttpPut("items/{itemId}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> UpdateItem(int consultationId, int itemId, [FromBody] PrescriptionItemUpdateDto dto, CancellationToken ct)
    {
        var updated = await _service.UpdatePrescriptionItemAsync(itemId, dto, ct);
        return Ok(updated);
    }

    // Delete item
    [HttpDelete("items/{itemId}")]
    [Authorize(Roles = "Doctor")]
    public async Task<IActionResult> DeleteItem(int consultationId, int itemId, CancellationToken ct)
    {
        await _service.DeletePrescriptionItemAsync(itemId, ct);
        return NoContent();
    }
}
