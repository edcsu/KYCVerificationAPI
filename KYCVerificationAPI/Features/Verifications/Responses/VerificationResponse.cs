using System.ComponentModel;

namespace KYCVerificationAPI.Features.Verifications.Responses;

public record VerificationResponse
{
    [Description("HTTP StatusCode of the verification")]
    public int StatusCode { get; init; }
    
    [Description("Unique identifier of the verification")]
    public Guid TransactionId { get; init; }

    [Description("The time the verification was created")]
    public DateTime CreatedAt { get; init; }
    
    [Description("The time the verification was updated")]
    public DateTime UpdatedAt { get; init; }

    public VerificationData Data { get; init; } = new VerificationData();
}