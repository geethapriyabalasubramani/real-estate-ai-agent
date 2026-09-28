using Asp.Versioning;
using Amazon.Runtime;
using Microsoft.AspNetCore.Hosting;
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
    private readonly IEmbeddingService _embeddingService;
    private readonly IWebHostEnvironment _environment;

    public AiController(
        IBedrockChatService bedrockChatService,
        IOptions<BedrockOptions> bedrockOptions,
        IEmbeddingService embeddingService,
        IWebHostEnvironment environment)
    {
        _bedrockChatService = bedrockChatService;
        _bedrockOptions = bedrockOptions.Value;
        _embeddingService = embeddingService;
        _environment = environment;
    }

    [HttpPost("embed")]
    public async Task<ActionResult<EmbedTextResponse>> Embed(
        [FromBody] EmbedTextRequest request,
        CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var vector = await _embeddingService.EmbedAsync(request.Text, cancellationToken);
            var preview = vector.ToArray().Take(5).ToArray();
            return Ok(new EmbedTextResponse(
                _bedrockOptions.EmbeddingModelId,
                vector.ToArray().Length,
                preview));
        }
        catch (AmazonServiceException ex)
        {
            return StatusCode(502, new { error = "Bedrock embedding failed.", detail = ex.Message });
        }
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
            var text = await _bedrockChatService.CompleteAsync(
                request.Prompt,
                cancellationToken: cancellationToken);
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
