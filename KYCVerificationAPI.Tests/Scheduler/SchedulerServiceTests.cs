using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Exceptions;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Scheduler.Services;
using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Responses;
using KYCVerificationAPI.Features.Vendors.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace KYCVerificationAPI.Tests.Scheduler;


public class SchedulerServiceTests
{
    private readonly Mock<IVerificationRepository> _verificationRepositoryMock;
    private readonly Mock<IExternalIdService> _externalIdServiceMock;
    private readonly SchedulerService _schedulerService;

    public SchedulerServiceTests(ITestOutputHelper testOutputHelper)
    {
        _verificationRepositoryMock = new Mock<IVerificationRepository>();
        _externalIdServiceMock = new Mock<IExternalIdService>();
        var logger = testOutputHelper.BuildLoggerFor<SchedulerService>();
        _schedulerService = new SchedulerService(
            _verificationRepositoryMock.Object,
            _externalIdServiceMock.Object,
            logger);
    }

    [Fact]
    public async Task ScheduleVerificationAsync_WhenVerificationNotFound_ReturnsFalse()
    {
        // Arrange
        var verificationId = Guid.NewGuid();
        _verificationRepositoryMock.Setup(x => x.GetByIdAsync(verificationId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Verification?)null);
        // Act
        var result = await _schedulerService.ScheduleVerificationAsync(verificationId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ScheduleVerificationAsync_WhenVerificationFound_ReturnsTrue()
    {
        // Arrange
        var verification = new Verification { Id = Guid.NewGuid() };
        var kycResponse = new KycResponse { KycStatus = KycStatus.Ok };
        
        _verificationRepositoryMock.Setup(x => x.GetByIdAsync(verification.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);
        _externalIdServiceMock.Setup(x => x.VerifyAsync(It.IsAny<KycRequest>(), 
                It.IsAny<MockMode>()))
            .ReturnsAsync(kycResponse);
        _verificationRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Verification>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);

        // Act
        var result = await _schedulerService.ScheduleVerificationAsync(verification.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ScheduleVerificationAsync_WhenKycStatusError_ThrowsException()
    {
        // Arrange
        var verification = TestHelpers.GetVerifications(1).First();
        verification.Retries = 0;
        var kycResponse = new KycResponse {
            KycStatus = KycStatus.Error,
            Message = "An error occured during verification"
        };
        
        _verificationRepositoryMock.Setup(x => x.GetByIdAsync(verification.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);
        
        _verificationRepositoryMock.Setup(x => x.UpdateAsync(verification, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);
        
        _externalIdServiceMock.Setup(x => x.VerifyAsync(It.IsAny<KycRequest>(), 
                It.IsAny<MockMode>()))
            .ReturnsAsync(kycResponse);

        // Act & Assert
        await Assert.ThrowsAsync<ClientFriendlyException>(() => 
            _schedulerService.ScheduleVerificationAsync(verification.Id));
    }

    [Fact]
    public Task HandleKycErrorResponse_WhenMaxRetriesReached_UpdatesStatusToFailed()
    {
        // Arrange
        var verification = TestHelpers.GetVerifications(1).First();
        verification.Retries = ApiConstants.MaxRetryAttempts;
        verification.Status = VerificationStatus.Failed;
        verification.KycStatus = KycStatus.Error;

        _verificationRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Verification>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);

        // Act & Assert
        Assert.Equal(VerificationStatus.Failed, verification.Status);
        Assert.Equal(KycStatus.Error, verification.KycStatus);
        return Task.CompletedTask;
    }
}