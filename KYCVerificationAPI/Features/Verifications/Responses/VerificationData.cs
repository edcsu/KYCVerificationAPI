using KYCVerificationAPI.Core;

namespace KYCVerificationAPI.Features.Verifications.Responses;

public record VerificationData
{
    public required string FirstName { get; init; }
    public required string GivenName { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string Nin { get; init; }
    public required string CardNumber { get; init; }
    public required VerificationStatus Status { get; init; }
    public required bool NameAsPerIdMatches { get; init; }
    public required bool NinAsPerIdMatches { get; init; }
    public required bool DateOfBirthMatches { get; init; }
}