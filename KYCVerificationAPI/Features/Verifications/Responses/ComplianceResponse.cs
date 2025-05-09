namespace KYCVerificationAPI.Features.Verifications.Responses;

public record ComplianceResponse
{
    public required string MadeBy { get; set; }
    
    public IEnumerable<VerificationResponse> VerificationResponses { get; set; } = [];
}