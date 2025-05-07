using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;

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
    
    public static VerificationResponse MapToVerificationResponse(this Verification request)
    {
        return new VerificationResponse
        {
            TransactionId = request.Id,
            StatusCode = 200,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.LastUpdated,
            CreatedBy = request.CreatedBy,
            Status = request.Status,
            Data = new VerificationData
            {
                Nin = request.Nin,
                CardNumber = request.CardNumber,
                FirstName = request.FirstName,
                GivenName = request.GivenName,
                DateOfBirth = request.DateOfBirth,
                KycStatus = request.KycStatus,
                NinAsPerIdMatches = request.NinAsPerIdMatches,
                NameAsPerIdMatches = request.NameAsPerIdMatches,
                DateOfBirthMatches = request.DateOfBirthMatches,
                CardNumberAsPerIdMatches = request.CardNumberAsPerIdMatches,
                Remarks = request.KycMessage
            }
        };
    }
}