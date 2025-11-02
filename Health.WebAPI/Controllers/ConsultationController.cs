using Health.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Health.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // optional: protect it with JWT
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateConsultation([FromBody] object healthValuesJson)
        {
            

            int userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return BadRequest("Invalid or missing user claim.");
            Console.WriteLine($"UserId from JWT: {userId}");

            var consultation = await _consultationService.CreateConsultationAsync(userId, healthValuesJson.ToString()!);
            return Ok(new
            {
                success = true,
                message = "Consultation created successfully",
                data = consultation
            });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetConsultationsByUser(int userId)
        {
            // later: create a method in service to retrieve consultations for a user
            return Ok(new { message = "Coming soon: list consultations" });
        }
    }
}
