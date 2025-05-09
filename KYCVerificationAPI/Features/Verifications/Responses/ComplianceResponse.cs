namespace KYCVerificationAPI.Features.Verifications.Responses;

public record ComplianceResponse
{
    public required string MadeBy { get; set; }
    
    public List<VerificationResponse> VerificationResponses { get; set; } = [];
}