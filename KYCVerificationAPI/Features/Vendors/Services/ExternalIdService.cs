using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Responses;

namespace KYCVerificationAPI.Features.Vendors.Services;

public class ExternalIdService : IExternalIdService
{
    public Task<KycResponse> VerifyAsync(KycRequest request)
    {
        throw new NotImplementedException();
    }
}