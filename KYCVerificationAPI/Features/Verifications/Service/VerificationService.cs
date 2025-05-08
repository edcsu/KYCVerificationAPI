using Hangfire;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Scheduler.Services;
using KYCVerificationAPI.Features.Verifications.Mappings;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;

namespace KYCVerificationAPI.Features.Verifications.Service;

public class VerificationService(IVerificationRepository verificationRepository,
    ISchedulerService schedulerService,  
    IBackgroundJobClient backgroundJob,
    ILogger<VerificationService> logger) : IVerificationService
{
    public async Task<VerificationResponse> CreateAsync(CreateVerification createVerification,
        string correlationId,
        string email,
        CancellationToken cancellationToken = default)
    {
        var verification = createVerification.MapToVerification();
        verification.CorrelationId = correlationId;
        verification.CreatedBy = email;
        
        var savedVerification = await verificationRepository.AddAsync(verification, cancellationToken);
        var verificationResponse = savedVerification.MapToVerificationResponse();
        logger.LogInformation("Saved verification");

        var verificationJobId = backgroundJob.Enqueue(() => schedulerService.ScheduleVerificationAsync(verification.Id, cancellationToken));
        logger.LogInformation("Scheduled verification request with jobID {JobId}", verificationJobId);
        return verificationResponse;
    }

    public async Task<VerificationResponse?> GetByIdAsync(Guid id, 
        CancellationToken cancellationToken = default)
    {
        var verification = await verificationRepository.GetByIdAsync(id, cancellationToken);
        return verification?.MapToVerificationResponse();
    }
}