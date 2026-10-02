namespace RealEstateAiAgent.Api.Contracts;

public record AgentChatResponse(
    string Reply,
    IReadOnlyList<AgentToolCallTrace> ToolCalls);
