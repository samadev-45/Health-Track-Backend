using Health.Application.Common;
using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Health.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileStorageController : ControllerBase
    {
        private readonly IFileStorageService _service;

        public FileStorageController(IFileStorageService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadFile(int id, CancellationToken ct)
        {
            try
            {
                var file = await _service.DownloadAsync(id, ct);

                return File(file.FileBytes, file.ContentType, file.FileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Unexpected error: " + ex.Message));
            }
        }
    }
}
