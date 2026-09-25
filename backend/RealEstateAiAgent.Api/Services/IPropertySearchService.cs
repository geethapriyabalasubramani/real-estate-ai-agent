using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Services;

public interface IPropertySearchService
{
    Task<PagedPropertyResponse> SearchAsync(
        PropertySearchQuery query,
        CancellationToken cancellationToken = default);
}
