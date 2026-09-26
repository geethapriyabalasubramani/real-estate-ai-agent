namespace RealEstateAiAgent.Api.Exceptions;

public sealed class AiCriteriaParseException : Exception
{
    public AiCriteriaParseException(string message) : base(message) { }
}

public sealed class AiCriteriaValidationException : Exception
{
    public AiCriteriaValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("AI-generated search criteria failed validation.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}