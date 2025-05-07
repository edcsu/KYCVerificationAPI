using Hangfire;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Scheduler.Services;
using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Services;
using KYCVerificationAPI.Features.Verifications.Mappings;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;

namespace KYCVerificationAPI.Features.Verifications.Service;

public class VerificationService(IVerificationRepository verificationRepository,
    ISchedulerService schedulerService,  
    IBackgroundJobClient _backgroundJob,
    ILogger<VerificationService> logger) : IVerificationService
{
    public async Task<Guid> CreateAsync(CreateVerification createVerification,
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        var verification = createVerification.MapToVerification();
        verification.CorrelationId = correlationId;
        
        await verificationRepository.Add(verification, cancellationToken);
        logger.LogInformation("Saved verification");

        var verificationJobId = _backgroundJob.Enqueue(() => schedulerService.ScheduleVerificationAsync(verification.Id, cancellationToken));
        logger.LogInformation("Scheduled verification request with jobID {JobId}", verificationJobId);
        return verification.Id;
    }

    public async Task<VerificationResponse?> GetByIdAsync(Guid id, 
        CancellationToken cancellationToken = default)
    {
        var verification = await verificationRepository.GetByIdAsync(id, cancellationToken);
        return verification?.MapToVerificationResponse();
    }
}