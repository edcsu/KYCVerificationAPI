using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Responses;

namespace KYCVerificationAPI.Features.Vendors.Services;

public interface IExternalIdService
{
    Task<KycResponse> VerifyAsync(KycRequest request);
}