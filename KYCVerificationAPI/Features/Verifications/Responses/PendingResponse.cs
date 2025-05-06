using System.ComponentModel;
using KYCVerificationAPI.Core;

namespace KYCVerificationAPI.Features.Verifications.Responses;

public record PendingResponse
{
    [Description("HTTP StatusCode of the verification")]
    public required int Code { get; init; }
    
    [Description("Unique identifier of the verification")]
    public required Guid TransactionId { get; init; }
    
    [Description("Status of the verification")]
    public required VerificationStatus Status { get; init; }
    
    [Description("The time the verification was created")]
    public required DateTime CreatedAt { get; init; }
}