using Amazon.Runtime;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Services;

namespace RealEstateAiAgent.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/ai/agent")]
public class AgentController : ControllerBase
{
    private readonly IRealEstateAgentService _agent;

    public AgentController(IRealEstateAgentService agent)
    {
        _agent = agent;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<AgentChatResponse>> Chat(
        [FromBody] AgentChatRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await _agent.ChatAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AmazonServiceException ex)
        {
            return StatusCode(502, new { error = "Bedrock request failed.", detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(502, new { error = "Agent processing failed.", detail = ex.Message });
        }
    }
}
