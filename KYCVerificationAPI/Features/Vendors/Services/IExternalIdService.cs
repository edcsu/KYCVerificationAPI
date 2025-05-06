using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Responses;
using KYCVerificationAPI.Features.Verifications.Requests;

namespace KYCVerificationAPI.Features.Vendors.Services;

public interface IExternalIdService
{
    Task<KycResponse> VerifyAsync(KycRequest request, MockMode mockMode);
}