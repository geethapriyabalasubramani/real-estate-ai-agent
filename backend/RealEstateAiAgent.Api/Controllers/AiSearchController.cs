using Asp.Versioning;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateAiAgent.Api.Contracts;
using RealEstateAiAgent.Api.Exceptions;
using RealEstateAiAgent.Api.Services;

namespace RealEstateAiAgent.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/ai")]
public class AiSearchController : ControllerBase
{
    private readonly INaturalLanguagePropertySearchService _naturalLanguageSearch;
    private readonly ISemanticPropertySearchService _semanticPropertySearch;
    private readonly IHybridPropertySearchService _hybridSearch;

    public AiSearchController(
        INaturalLanguagePropertySearchService naturalLanguageSearch,
        ISemanticPropertySearchService semanticPropertySearch,
        IHybridPropertySearchService hybridSearch)
    {
        _naturalLanguageSearch = naturalLanguageSearch;
        _semanticPropertySearch = semanticPropertySearch;
        _hybridSearch = hybridSearch;
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
        catch (AiCriteriaValidationException ex)
        {
            var problem = new ValidationProblemDetails(ex.Errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "AI-generated criteria failed validation.",
                Detail = ex.Message,
            };
            return ValidationProblem(problem);
        }
        catch (AiCriteriaParseException ex)
        {
            return BadRequest(new { error = ex.Message });
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

    [HttpPost("semantic-search")]
    public async Task<ActionResult<SemanticPropertySearchResponse>> SemanticSearch(
        [FromBody] SemanticPropertySearchRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await _semanticPropertySearch.SearchAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AmazonServiceException ex)
        {
            return StatusCode(502, new { error = "Bedrock embedding failed.", detail = ex.Message });
        }
    }

    [HttpPost("hybrid-search")]
    public async Task<ActionResult<HybridPropertySearchResponse>> HybridSearch(
        [FromBody] HybridPropertySearchRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await _hybridSearch.SearchAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AmazonServiceException ex)
        {
            return StatusCode(502, new { error = "Bedrock request failed.", detail = ex.Message });
        }
    }
}

