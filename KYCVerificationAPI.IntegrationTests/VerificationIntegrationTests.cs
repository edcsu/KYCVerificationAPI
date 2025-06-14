using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;
using Microsoft.AspNetCore.Mvc;

namespace KYCVerificationAPI.IntegrationTests;

[Collection(nameof(VerificationTestCollection))]
public class VerificationIntegrationTests : IClassFixture<KycWebApplicationFactory>, IAsyncLifetime
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
    
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _factory.SharedFixture.ResetDatabaseAsync();

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
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        validationResult?.Errors.Count.ShouldBe(2);
        validationResult?.Title.ShouldBe("One or more validation errors occurred.");
        
        var error = validationResult?.Errors.FirstOrDefault();
        error?.Key.ShouldBe("$");
        error?.Value.First().ShouldBe("JSON deserialization for type 'KYCVerificationAPI.Features.Verifications.Requests.CreateVerification' was missing required properties including: 'firstName', 'givenName', 'dateOfBirth', 'nin', 'cardNumber'.");
    }
    
    [Fact]
    public async Task CreateVerification_ShouldReturnBadRequest_WhenCardNumberIsInvalid()
    {
        // Arrange
        var invalidCardNumber = string.Empty;
        var invalidRequest = new CreateVerification
        {
            FirstName = "Jane",
            GivenName = "Doe",
            DateOfBirth = DateOnly.Parse("2012-02-02"),
            CardNumber = invalidCardNumber,
            Nin = "12345678901234"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/verifications", invalidRequest);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        validationResult?.Errors.Count.ShouldBe(1);
        validationResult?.Title.ShouldBe("One or more validation errors occurred.");
        
        var error = validationResult?.Errors.FirstOrDefault();
        error?.Key.ShouldBe("CardNumber");
        error?.Value.First().ShouldBe("The field CardNumber must be a string or collection type with a minimum length of '9' and maximum length of '9'.");
    }
    
    [Fact]
    public async Task CreateVerification_ShouldReturnBadRequest_WhenNinIsInvalid()
    {
        // Arrange
        var invalidNin = "1234";
        var invalidRequest = new CreateVerification
        {
            FirstName = "Jane",
            GivenName = "Doe",
            DateOfBirth = DateOnly.Parse("2012-02-02"),
            CardNumber = "123456789",
            Nin = invalidNin
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/verifications", invalidRequest);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        validationResult?.Errors.Count.ShouldBe(1);
        validationResult?.Title.ShouldBe("One or more validation errors occurred.");
        
        var error = validationResult?.Errors.FirstOrDefault();
        error?.Key.ShouldBe("Nin");
        error?.Value.First().ShouldBe("The field Nin must be a string or collection type with a minimum length of '14' and maximum length of '14'.");
    }
    
    [Fact]
    public async Task CreateVerification_ShouldReturnBadRequest_WhenFirstNameIsInvalid()
    {
        // Arrange
        var invalidFirstName = "J";
        var invalidRequest = new CreateVerification
        {
            FirstName = invalidFirstName,
            GivenName = "Doe",
            DateOfBirth = DateOnly.Parse("2012-02-02"),
            CardNumber = "123456789",
            Nin = "12345678901234"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/verifications", invalidRequest);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        validationResult?.Errors.Count.ShouldBe(1);
        validationResult?.Title.ShouldBe("One or more validation errors occurred.");
        
        var error = validationResult?.Errors.FirstOrDefault();
        error?.Key.ShouldBe("FirstName");
        error?.Value.First().ShouldBe("The field FirstName must be a string or collection type with a minimum length of '2' and maximum length of '100'.");
    }
    
    [Fact]
    public async Task CreateVerification_ShouldReturnBadRequest_WhenGivenNameIsInvalid()
    {
        // Arrange
        var invalidGivenName = "E";
        var invalidRequest = new CreateVerification
        {
            FirstName = "Jane",
            GivenName = invalidGivenName,
            DateOfBirth = DateOnly.Parse("2012-02-02"),
            CardNumber = "123456789",
            Nin = "12345678901234"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/verifications", invalidRequest);
        var validationResult = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        validationResult?.Errors.Count.ShouldBe(1);
        validationResult?.Title.ShouldBe("One or more validation errors occurred.");
        
        var error = validationResult?.Errors.FirstOrDefault();
        error?.Key.ShouldBe("GivenName");
        error?.Value.First().ShouldBe("The field GivenName must be a string or collection type with a minimum length of '2' and maximum length of '100'.");
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
        results.TotalItems.ShouldBe(2);
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
