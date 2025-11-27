using Health.Application.Common;
using Health.Application.DTOs;
using Health.Application.DTOs.Auth;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/patient/profile")]
[ApiController]
[Authorize(Roles = "Patient")]
public class PatientProfileController : ControllerBase
{
    private readonly IProfileService _service;

    public PatientProfileController(IProfileService service)
    {
        _service = service;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await _service.GetPatientProfileAsync(CurrentUserId, ct);
        return Ok(ApiResponse<PatientProfileDto>.SuccessResponse(result));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePatientProfileDto dto, CancellationToken ct)
    {
        var result = await _service.UpdatePatientProfileAsync(CurrentUserId, dto, ct);
        return Ok(ApiResponse<PatientProfileDto>.SuccessResponse(result, "Profile updated."));
    }
}
