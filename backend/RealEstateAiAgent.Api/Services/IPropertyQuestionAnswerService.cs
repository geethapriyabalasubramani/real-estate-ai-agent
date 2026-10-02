using RealEstateAiAgent.Api.Contracts;

namespace RealEstateAiAgent.Api.Services;

public interface IPropertyQuestionAnswerService
{
    Task<PropertyAskResponse> AskAsync(
        PropertyAskRequest request,
        CancellationToken cancellationToken = default);
}