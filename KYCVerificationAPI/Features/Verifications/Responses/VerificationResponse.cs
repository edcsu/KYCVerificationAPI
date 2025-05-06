namespace KYCVerificationAPI.Features.Verifications.Responses;

public record VerificationResponse
{
    public required int StatusCode { get; init; }
    public required Guid TransactionId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required VerificationData Data { get; init; }
}