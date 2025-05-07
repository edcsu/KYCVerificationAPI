using Hangfire;
using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Responses;

namespace KYCVerificationAPI.Features.Vendors.Services;

public class ExternalIdService : IExternalIdService
{
    private readonly ILogger<ExternalIdService> _logger;

    public ExternalIdService(ILogger<ExternalIdService> logger)
    {
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 5, DelaysInSeconds = [2, 1, 3, 2, 4])]
    public async Task<KycResponse> VerifyAsync(KycRequest request, MockMode mockMode)
    {
        switch (mockMode)
        {
            case MockMode.Success:
            default:
                _logger.LogInformation("Verification was a success");
                await Task.Delay(1000);
                return new KycResponse
                {
                    KycStatus = KycStatus.Ok,
                    NameAsPerIdMatches = true,
                    NinAsPerIdMatches = true,
                    DateOfBirthMatches = true,
                    CardNumberAsPerIdMatches = true,
                    Message = "Verification was successful"
                };
            
            case MockMode.Failed:
                _logger.LogInformation("Verification failed");
                var randomFailedValue = EnumHelper.GetRandomEnumValue<FailedValue>();
                await Task.Delay(3000);
                switch (randomFailedValue)
                {
                    case FailedValue.Nin:
                    default:
                        return new KycResponse
                        {
                            KycStatus = KycStatus.Failed,
                            NameAsPerIdMatches = true,
                            NinAsPerIdMatches = false,
                            DateOfBirthMatches = true,
                            CardNumberAsPerIdMatches = true,
                            Message = "Verification failed due to nin mismatch"
                        };
                    
                    case FailedValue.Name:
                        return new KycResponse
                        {
                            KycStatus = KycStatus.Failed,
                            NameAsPerIdMatches = false,
                            NinAsPerIdMatches = true,
                            DateOfBirthMatches = true,
                            CardNumberAsPerIdMatches = true,
                            Message = "Verification failed due to name mismatch"
                        };
                    
                    case FailedValue.Birth:
                        return new KycResponse
                        {
                            KycStatus = KycStatus.Failed,
                            NameAsPerIdMatches = true,
                            NinAsPerIdMatches = true,
                            DateOfBirthMatches = false,
                            CardNumberAsPerIdMatches = true,
                            Message = "Verification failed due to date of birth mismatch"
                        };
                    
                    case FailedValue.CardNumber:
                        return new KycResponse
                        {
                            KycStatus = KycStatus.Failed,
                            NameAsPerIdMatches = true,
                            NinAsPerIdMatches = true,
                            DateOfBirthMatches = true,
                            CardNumberAsPerIdMatches = false,
                            Message = "Verification failed due to card number mismatch"
                        };
                }
            
            case MockMode.Error:
                _logger.LogInformation("An error occured during verification");
                await Task.Delay(3000);
                return new KycResponse
                {
                    KycStatus = KycStatus.Error,
                    Message = "An error occured during verification"
                };
        }
    }
}