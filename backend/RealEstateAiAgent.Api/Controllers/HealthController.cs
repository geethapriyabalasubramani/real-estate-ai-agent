using Microsoft.AspNetCore.Mvc;

namespace RealEstateAiAgent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "RealEstateAiAgent.Api",
            utc = DateTime.UtcNow
        });
    }
}