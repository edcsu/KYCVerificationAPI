using KYCVerificationAPI.Core;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;

namespace KYCVerificationAPI.Data.Repositories;

public interface IVerificationRepository : IGenericRepository<Verification>
{
    Task<PagedResult<VerificationResponse>> GetHistoryAsync(VerificationFilter verificationFilter,
        string userEmail,
        CancellationToken cancellationToken = default);
}