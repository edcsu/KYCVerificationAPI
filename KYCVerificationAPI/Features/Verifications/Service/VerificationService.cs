using System.Net.Mime;
using Hangfire;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Scheduler.Services;
using KYCVerificationAPI.Features.Verifications.Mappings;
using KYCVerificationAPI.Features.Verifications.Reports.Templates;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;
using QuestPDF.Fluent;

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

    public async Task<PagedResult<VerificationResponse>> GetHistoryAsync(VerificationFilter verificationFilter, 
        string userEmail,
        CancellationToken cancellationToken = default)
    {
        var response = await verificationRepository.GetHistoryAsync(verificationFilter, userEmail, cancellationToken);
        return response;
    }

    public async Task<FileViewModel> GetComplianceFileAsync(string userEmail,
        CancellationToken cancellationToken = default)
    {
        var allVerifications = await verificationRepository.GetAllAsync(cancellationToken);
        var verificationResponses = allVerifications.Select(item => item.MapToVerificationResponse());
        var complianceResponse = new ComplianceResponse()
        {
            MadeBy = userEmail,
            VerificationResponses = verificationResponses
        };
        var complianceDocument = new ComplianceDocument(complianceResponse);
        return new FileViewModel
        {
            Contents = complianceDocument.GeneratePdf(),
            ContentType = MediaTypeNames.Application.Pdf,
            Name = $"compliance_report{DateTime.Now:yyyyMMddHHmmss}.pdf",
        };
    }
}