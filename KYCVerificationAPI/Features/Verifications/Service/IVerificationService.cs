using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;

namespace KYCVerificationAPI.Features.Verifications.Service;

public interface IVerificationService
{
    Task<Guid> CreateAsync(CreateVerification createVerification, CancellationToken cancellationToken = default);
    
    Task<VerificationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}