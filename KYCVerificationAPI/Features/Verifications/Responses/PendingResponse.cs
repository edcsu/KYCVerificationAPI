using KYCVerificationAPI.Core;

namespace KYCVerificationAPI.Features.Verifications.Responses;

public record PendingResponse
{
    public required int StatusCode { get; init; }
    public required Guid TransactionId { get; init; }
    public required VerificationStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
}