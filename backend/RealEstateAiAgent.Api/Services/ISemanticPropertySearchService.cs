using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Services;

public interface ISemanticPropertySearchService
{
    Task<SemanticPropertySearchResponse> SearchAsync(
        SemanticPropertySearchRequest request,
        CancellationToken cancellationToken = default);
}
