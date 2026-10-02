namespace RealEstateAiAgent.Api.Contracts;

public record PropertyAskResponse(
    string Question,
    string Answer,
    IReadOnlyList<PropertyAskSourceItem> Sources);