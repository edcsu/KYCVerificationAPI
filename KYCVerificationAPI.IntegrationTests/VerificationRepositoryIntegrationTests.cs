using KYCVerificationAPI.Data;
using KYCVerificationAPI.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace KYCVerificationAPI.IntegrationTests;

public class VerificationRepositoryIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private VerificationRepository _verificationRepository;
    
    public VerificationRepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _verificationRepository = new VerificationRepository(_fixture.DbContext);
    }
    
    [Fact]
    public async Task Add_ShouldAddNewVerification()
    {
        // Arrange
        var newVerification = IntegrationHelpers.GetVerification();
        
        // Act
        var result = await _verificationRepository.Add(newVerification);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newVerification, result);
    }
    
    [Fact]
    public async Task Update_ShouldUpdateExistingVerification()
    {
        // Arrange
        var newVerification = IntegrationHelpers.GetVerification();
        var savedVerification = await _verificationRepository.Add(newVerification);

        const string newMessage = "The KYC is valid";
        savedVerification.KycMessage = newMessage;
        // Act
        var result = await _verificationRepository.Update(savedVerification);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newMessage, result.KycMessage);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnVerification_WhenVerificationExists()
    {
        // Arrange
        var expectedVerification = IntegrationHelpers.GetVerification();
        await _verificationRepository.Add(expectedVerification);

        // Act
        var result = await _verificationRepository.GetByIdAsync(expectedVerification.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedVerification, result);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnAllVerifications()
    {
        // Arrange
        var newVerification = IntegrationHelpers.GetVerification();
        var savedVerification = await _verificationRepository.Add(newVerification);

        // Act
        var result = await _verificationRepository.GetAll(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single( result);
    }
}