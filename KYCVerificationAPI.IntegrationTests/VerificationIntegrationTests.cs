using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Features.Verifications.Responses;

namespace KYCVerificationAPI.IntegrationTests;

[Collection(nameof(VerificationTestCollection))]
public class VerificationIntegrationTests : IClassFixture<KycWebApplicationFactory>
{
    private readonly KycWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public VerificationIntegrationTests(KycWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.HttpClient;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
            IntegrationHelpers.GetToken());
    }

    [Fact]
    public async Task CreateVerification_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        var request = IntegrationHelpers.GetCreateVerification();

        // Act
        var response = await _client.PostAsJsonAsync("/api/verifications", request);
        var result = await response.Content.ReadFromJsonAsync<PendingResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Status.ShouldBe(VerificationStatus.Pending);
        result.TransactionId.ShouldNotBe(Guid.Empty);
        
        // Verify database state
        var verification = await _factory.SharedFixture.DbContext.Verifications
            .FirstOrDefaultAsync(v => v.Id == result.TransactionId);
        verification.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetVerification_ShouldReturnNotFound_WhenVerificationDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/verifications/{nonExistentId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVerification_ShouldReturnVerification_WhenExists()
    {
        // Arrange
        var createRequest = IntegrationHelpers.GetCreateVerification();
        var createResponse = await _client.PostAsJsonAsync("/api/verifications", createRequest);
        var pendingResponse = await createResponse.Content.ReadFromJsonAsync<PendingResponse>();
        pendingResponse.ShouldNotBeNull();

        // Act
        var response = await _client.GetAsync($"/api/verifications/{pendingResponse.TransactionId}");
        var result = await response.Content.ReadFromJsonAsync<VerificationResponse>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.TransactionId.ShouldBe(pendingResponse.TransactionId);
    }

    [Fact]
    public async Task CreateVerification_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        // Arrange
        var invalidRequest = new { }; // Empty anonymous object as invalid request

        // Act
        var response = await _client.PostAsJsonAsync("/api/verifications", invalidRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetVerifications_ShouldReturnAllVerifications()
    {
        // Arrange
        // Create multiple verifications
        var request1 = IntegrationHelpers.GetCreateVerification();
        var request2 = IntegrationHelpers.GetCreateVerification();
        
        await _client.PostAsJsonAsync("/api/verifications", request1);
        await _client.PostAsJsonAsync("/api/verifications", request2);

        // Act
        var response = await _client.GetAsync("/api/verifications");
        var results = await response.Content.ReadFromJsonAsync<PagedResult<VerificationResponse>>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        results.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateVerification_DatabasePersistence_Test()
    {
        // Arrange
        var request = IntegrationHelpers.GetCreateVerification();

        // Act
        var response = await _client.PostAsJsonAsync("/api/verifications", request);
        var result = await response.Content.ReadFromJsonAsync<PendingResponse>();
        result.ShouldNotBeNull();

        // Assert - Check Database State
        var dbVerification = await _factory.SharedFixture.DbContext.Verifications
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == result.TransactionId);

        dbVerification.ShouldNotBeNull();
        dbVerification.Status.ShouldBe(VerificationStatus.Pending);
        dbVerification.CreatedAt.ShouldBeInRange(
            DateTime.UtcNow.AddMinutes(-1), 
            DateTime.UtcNow);
    }
}
