namespace KYCVerificationAPI.Features.Verifications.Requests;

public record CreateVerification{
    public required string FirstName { get; init; }
    public required string GivenName { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string Nin { get; init; }
    public required string CardNumber { get; init; }
}