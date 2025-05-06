using KYCVerificationAPI.Core;

namespace KYCVerificationAPI.Features.Verifications.Responses;

public record VerificationData
{
    public required VerificationStatus Status { get; init; }
    public required bool NameAsPerIdMatches { get; init; }
    public required bool NinAsPerIdMatches { get; init; }
    public required bool DateOfBirthMatches { get; init; }
}