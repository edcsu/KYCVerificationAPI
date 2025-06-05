using System.Net.Mime;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Scheduler.Services;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;
using KYCVerificationAPI.Features.Verifications.Service;
using Microsoft.Extensions.Logging;
using Moq;
using QuestPDF.Infrastructure;
using Xunit.Abstractions;

namespace KYCVerificationAPI.Tests.Services;

public class VerificationServiceTests
{
    private readonly Mock<IVerificationRepository> _mockRepository;
    private readonly Mock<IBackgroundJobClient> _mockBackgroundJob;
    private readonly VerificationService _service;
    private const string CorrelationId = "test-correlation-id";
    private const string Email = "test@example.com";
    
    public VerificationServiceTests(ITestOutputHelper testOutputHelper)
    {
        _mockRepository = new Mock<IVerificationRepository>();
        var mockSchedulerService = new Mock<ISchedulerService>();
        _mockBackgroundJob = new Mock<IBackgroundJobClient>();
        ILogger<VerificationService> logger = testOutputHelper.BuildLoggerFor<VerificationService>();
        
        _service = new VerificationService(
            _mockRepository.Object,
            mockSchedulerService.Object,
            _mockBackgroundJob.Object,
            logger
        );
        
        _mockBackgroundJob.Setup(x => x.Create(It.Is<Job>(
                job => job.Method.Name == nameof(ISchedulerService.ScheduleVerificationAsync)), 
                It.IsAny<EnqueuedState>()))
            .Returns("1");
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateVerificationAndScheduleJob()
    {
        // Arrange
        var createVerification = TestHelpers.GetCreateVerification();
        var verification = TestHelpers.GetVerifications(1).First();

        _mockRepository.Setup(r => r.AddAsync(
            It.IsAny<Verification>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);

        // Act
        var result = await _service.CreateAsync(createVerification, CorrelationId, Email);

        // Assert
        _mockRepository.Verify(r => r.AddAsync(
            It.Is<Verification>(v => 
                v.CorrelationId == CorrelationId && 
                v.CreatedBy == Email),
            It.IsAny<CancellationToken>()), 
            Times.Once);

        _mockBackgroundJob.Verify(x => x.Create(
            It.Is<Job>(y =>
                y.Method.Name == nameof(ISchedulerService.ScheduleVerificationAsync)),
            It.IsAny<EnqueuedState>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnVerificationResponse_WhenVerificationExists()
    {
        // Arrange
        var verification = TestHelpers.GetVerifications(1).First();

        _mockRepository.Setup(r => r.GetByIdAsync(verification.Id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(verification);

        // Act
        var result = await _service.GetByIdAsync(verification.Id);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetByIdAsync(verification.Id,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenVerificationDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(id, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Verification?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetHistoryAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var filter = new VerificationFilter();
        var expectedResult = new PagedResult<VerificationResponse>();

        _mockRepository.Setup(r => r.GetHistoryAsync(filter, 
                Email,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.GetHistoryAsync(filter, Email);

        // Assert
        Assert.Same(expectedResult, result);
        _mockRepository.Verify(r => r.GetHistoryAsync(filter,
            Email,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetComplianceFileAsync_ShouldReturnFileViewModel()
    {
        // Arrange
        var verifications = TestHelpers.GetVerifications(20);

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(verifications);
        QuestPDF.Settings.License = LicenseType.Community; 
        // Act
        var result = await _service.GetComplianceFileAsync(Email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(MediaTypeNames.Application.Pdf, result.ContentType);
        Assert.Contains("compliance_report", result.Name);
        Assert.NotNull(result.Contents);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}
