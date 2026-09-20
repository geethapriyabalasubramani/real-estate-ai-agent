namespace RealEstateAiAgent.Api.Contracts;

public record PagedPropertyResponse(
    IReadOnlyList<PropertyListItemDto> Items,
    int Page,
    int PageSize,
    int TotalCount
);