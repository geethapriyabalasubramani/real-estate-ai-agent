using Microsoft.AspNetCore.Mvc;
using RealEstateAiAgent.Api.Data;
using Microsoft.EntityFrameworkCore;
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

    [HttpGet("db")]
    public async Task<IActionResult> GetDatabase(
        [FromServices] ApplicationDbContext db,
        [FromServices] IWebHostEnvironment env,
        CancellationToken cancellationToken)
    {
        try
        {
            var canConnect = await db.Database.CanConnectAsync(cancellationToken);

            if (!canConnect)
            {
                return StatusCode(503, new { status = "error", database = "unreachable" });
            }

            return Ok(new { status = "ok", database = "connected" });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                status = "error",
                database = "connection_failed",
                detail = env.IsDevelopment() ? ex.Message : null
            });
        }
    }
}