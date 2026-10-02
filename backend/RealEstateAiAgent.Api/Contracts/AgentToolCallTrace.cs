namespace RealEstateAiAgent.Api.Contracts;

public record AgentToolCallTrace(
    string ToolName,
    string ArgumentsJson,
    string ResultSummary);
