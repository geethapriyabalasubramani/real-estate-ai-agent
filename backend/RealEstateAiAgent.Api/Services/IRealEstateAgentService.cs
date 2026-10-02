using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Services;

public interface IRealEstateAgentService
{
    Task<AgentChatResponse> ChatAsync(
        AgentChatRequest request,
        CancellationToken cancellationToken = default);
}
