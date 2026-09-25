using Asp.Versioning;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAiAgent.Api.Configuration;
using RealEstateAiAgent.Api.Contracts;
using Microsoft.Extensions.Options;
using RealEstateAiAgent.Api.Services;

namespace RealEstateAiAgent.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ai")]
public class AiController : ControllerBase
{
    private readonly IBedrockChatService _bedrockChatService;
    private readonly BedrockOptions _bedrockOptions;

    public AiController(IBedrockChatService bedrockChatService, IOptions<BedrockOptions> bedrockOptions)
    {
        _bedrockChatService = bedrockChatService;
        _bedrockOptions = bedrockOptions.Value;
    }

    [HttpPost("complete")]
    public async Task<ActionResult<AiCompleteResponse>> Complete(
        [FromBody] AiCompleteRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var text = await _bedrockChatService.CompleteAsync(request.Prompt, cancellationToken);
            return Ok(new AiCompleteResponse(text, _bedrockOptions.ModelId));
        }
        catch (AmazonServiceException ex)
        {
            return StatusCode(502, new { error = "Bedrock request failed.", detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { error = ex.Message });
        }
    }
}
