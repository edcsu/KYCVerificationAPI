using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Exceptions;
using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Responses;
using KYCVerificationAPI.Features.Vendors.Services;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;

namespace KYCVerificationAPI.Features.Scheduler.Services;

public class SchedulerService : ISchedulerService
{
    private readonly IVerificationRepository _verificationRepository;
    private readonly IExternalIdService _externalIdService;
    private readonly ILogger<SchedulerService> _logger;

    public SchedulerService(IVerificationRepository verificationRepository, 
        IExternalIdService externalIdService, 
        ILogger<SchedulerService> logger)
    {
        _verificationRepository = verificationRepository;
        _externalIdService = externalIdService;
        _logger = logger;
    }

    public async Task<bool> ScheduleVerificationAsync(Guid verificationId, 
        CancellationToken cancellationToken = default)
    {
        var request = await _verificationRepository.GetByIdAsync(verificationId, cancellationToken);
        if (request is null)
        {
            _logger.LogError("Could not find verification with id: {VerificationId}", verificationId);
            return false;
        }
        
        ILogEventEnricher[] enrichers =
        {
            new PropertyEnricher("TransactionId", request.Id),
            new PropertyEnricher("CorrelationId", request.CorrelationId)
        };

        using (LogContext.Push(enrichers))
        {
            var kycRequest = new KycRequest
            {
                Nin = request.Nin,
                CardNumber = request.CardNumber,
                FirstName = request.FirstName,
                GivenName = request.GivenName,
                DateOfBirth = request.DateOfBirth,
            };
            
            var mockMode = EnumHelper.GetRandomEnumValue<MockMode>();
            
            var kycResponse = await _externalIdService.VerifyAsync(kycRequest, mockMode);

            if (kycResponse.KycStatus == KycStatus.Error)
            {
                await HandleKycErrorResponse(request, cancellationToken);
            }
            
            request.KycStatus = kycResponse.KycStatus;
            request.CardNumberAsPerIdMatches = kycResponse.CardNumberAsPerIdMatches;
            request.DateOfBirthMatches = kycResponse.DateOfBirthMatches;
            request.NameAsPerIdMatches = kycResponse.NameAsPerIdMatches;
            request.NinAsPerIdMatches = kycResponse.NinAsPerIdMatches;
            request.KycMessage = kycResponse.Message;
            var verificationRequest = await _verificationRepository.Update(request, cancellationToken);
            _logger.LogInformation("Finalised verification");

            return true;
        }
    }
    
    /// <summary>
    /// Handles KYC verification error response and updates the request status accordingly
    /// </summary>
    /// <returns>Updated verification request</returns>
    /// <exception cref="ClientFriendlyException">Thrown when KYC verification fails</exception>
    private async Task HandleKycErrorResponse(Verification request, CancellationToken cancellationToken)
    {
        if (request.Retries >= ApiConstants.MaxRetryAttempts)
        {
            request.Status = VerificationStatus.Invalid;
            request.KycStatus = KycStatus.Error;
            var verificationRequest = await _verificationRepository.Update(request, cancellationToken);
            _logger.LogError("The verification request with ID {RequestId} has failed after {MaxRetries} retries", 
                request.Id, ApiConstants.MaxRetryAttempts);
        }
        else
        {
            request.Retries++;
            var verificationRequest = await _verificationRepository.Update(request, cancellationToken);
            _logger.LogError("KYC verification failed. Scheduling retry {CurrentRetry} out of {MaxRetries} for request {RequestId}", 
                verificationRequest.Retries, ApiConstants.MaxRetryAttempts, request.Id);
        }
    
        throw new ClientFriendlyException("Failed to verify KYC data");
    }
}