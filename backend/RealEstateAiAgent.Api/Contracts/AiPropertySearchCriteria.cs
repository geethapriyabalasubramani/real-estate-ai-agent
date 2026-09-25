namespace RealEstateAiAgent.Api.Contracts;

public class AiPropertySearchCriteria
{
    public string? City { get; set; }
    public int? Bedrooms { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? HasGarage { get; set; }
}