using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Features.Vendors.Requests;
using KYCVerificationAPI.Features.Vendors.Responses;
using KYCVerificationAPI.Features.Vendors.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace KYCVerificationAPI.Tests.Vendor;

public class ExternalIdServiceTests
{
    private readonly ExternalIdService _externalIdService;

    public ExternalIdServiceTests()
    {
        var loggerMock = new Mock<ILogger<ExternalIdService>>();
        _externalIdService = new ExternalIdService(loggerMock.Object);
    }

    [Fact]
    public async Task VerifyAsync_WhenMockModeIsSuccess_ReturnsSuccessResponse()
    {
        // Arrange
        var request = TestHelpers.GetKycRequest();
        const MockMode mockMode = MockMode.Success;

        // Act
        var result = await _externalIdService.VerifyAsync(request, mockMode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(KycStatus.Ok, result.KycStatus);
        Assert.True(result.NameAsPerIdMatches);
        Assert.True(result.NinAsPerIdMatches);
        Assert.True(result.DateOfBirthMatches);
        Assert.True(result.CardNumberAsPerIdMatches);
        Assert.Equal("Verification was successful", result.Message);
    }

    [Fact]
    public async Task VerifyAsync_WhenMockModeIsError_ReturnsErrorResponse()
    {
        // Arrange
        var request = TestHelpers.GetKycRequest();
        var mockMode = MockMode.Error;

        // Act
        var result = await _externalIdService.VerifyAsync(request, mockMode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(KycStatus.Error, result.KycStatus);
        Assert.Equal("An error occured during verification", result.Message);
    }

    [Fact]
    public async Task VerifyAsync_WhenMockModeIsFailed_ReturnsCorrectFailureResponse()
    {
        // Arrange
        var request = TestHelpers.GetKycRequest();
        const MockMode mockMode = MockMode.Failed;

        // Act
        var result = await _externalIdService.VerifyAsync(request, mockMode);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(KycStatus.Failed, result.KycStatus);
    }
}