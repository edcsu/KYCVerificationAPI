using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Verifications.Requests;

namespace KYCVerificationAPI.Features.Verifications.Mappings;

public static class ContractMapping
{
    public static Verification MapToVerification(this CreateVerification request)
    {
        return new Verification
        {
            Id = Guid.CreateVersion7(),
            FirstName = request.FirstName,
            GivenName = request.GivenName,
            Nin = request.Nin,
            CardNumber = request.CardNumber,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow
        };
    }
}