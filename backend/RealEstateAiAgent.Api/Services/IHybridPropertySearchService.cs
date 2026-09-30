using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Services;

public interface IHybridPropertySearchService
{
    Task<HybridPropertySearchResponse> SearchAsync(
        HybridPropertySearchRequest request,
        CancellationToken cancellationToken = default);
}