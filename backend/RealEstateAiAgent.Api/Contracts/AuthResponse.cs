namespace RealEstateAiAgent.Api.Contracts;

public record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    string Email
);