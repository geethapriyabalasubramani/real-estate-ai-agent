using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Services;

public interface INaturalLanguagePropertySearchService
{
    Task<NaturalLanguageSearchResponse> SearchAsync(
        NaturalLanguageSearchRequest request,
        CancellationToken cancellationToken = default);
}
