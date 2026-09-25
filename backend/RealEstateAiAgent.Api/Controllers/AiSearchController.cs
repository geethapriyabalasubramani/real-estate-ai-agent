using Asp.Versioning;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Services;

namespace RealEstateAiAgent.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/ai")]
public class AiSearchController : ControllerBase
{
    private readonly INaturalLanguagePropertySearchService _naturalLanguageSearch;

    public AiSearchController(INaturalLanguagePropertySearchService naturalLanguageSearch)
    {
        _naturalLanguageSearch = naturalLanguageSearch;
    }

    [HttpPost("search")]
    public async Task<ActionResult<NaturalLanguageSearchResponse>> Search(
        [FromBody] NaturalLanguageSearchRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await _naturalLanguageSearch.SearchAsync(request, cancellationToken);
            return Ok(response);
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
