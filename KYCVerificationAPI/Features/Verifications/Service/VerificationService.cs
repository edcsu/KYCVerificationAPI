using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Services;
using KYCVerificationAPI.Features.Verifications.Mappings;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;

namespace KYCVerificationAPI.Features.Verifications.Service;

public class VerificationService(IVerificationRepository verificationRepository, 
    ILogger<VerificationService> logger) : IVerificationService
{
    public async Task<Guid> CreateAsync(CreateVerification createVerification, 
        CancellationToken cancellationToken = default)
    {
        var verification = createVerification.MapToVerification();
        await verificationRepository.Add(verification, cancellationToken);
        logger.LogInformation("Saved verification");

        var kycRequest = new KycRequest
        {
            FirstName = createVerification.FirstName,
            GivenName = createVerification.GivenName,
            DateOfBirth = createVerification.DateOfBirth,
            Nin = createVerification.Nin,
            CardNumber = createVerification.CardNumber
        };
        return verification.Id;
    }

    public async Task<VerificationResponse?> GetByIdAsync(Guid id, 
        CancellationToken cancellationToken = default)
    {
        return new VerificationResponse
        {
            TransactionId = Guid.CreateVersion7(),
            StatusCode = 200,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Data = new VerificationData
            {
                Nin =  "XXXPX1234A",
                CardNumber = "00000001",
                FirstName = "John",
                GivenName = "Doe",
                DateOfBirth = new DateOnly(2000,1,1),
                Status = VerificationStatus.Valid,
                NinAsPerIdMatches = true,
                NameAsPerIdMatches = true,
                DateOfBirthMatches = true,
            }
        };
    }
}