using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;

namespace KYCVerificationAPI.Features.Verifications.Service;

public interface IVerificationService
{
    Task<VerificationResponse> CreateAsync(CreateVerification createVerification, string correlationId,
        string email,
        CancellationToken cancellationToken = default);
    
    Task<VerificationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}