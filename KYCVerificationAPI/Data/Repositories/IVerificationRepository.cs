using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Verifications.Requests;

namespace KYCVerificationAPI.Data.Repositories;

public interface IVerificationRepository : IGenericRepository<Verification>
{
    Task<IEnumerable<Verification>> GetHistoryAsync(VerificationFilter verificationFilter,
        string userEmail,
        CancellationToken cancellationToken = default);
}