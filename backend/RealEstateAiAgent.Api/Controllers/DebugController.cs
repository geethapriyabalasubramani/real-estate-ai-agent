using Microsoft.AspNetCore.Mvc;

namespace RealEstateAiAgent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public DebugController(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpGet("throw")]
    public IActionResult Throw()
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        throw new InvalidOperationException("Simulated failure for Problem Details test.");
    }
}