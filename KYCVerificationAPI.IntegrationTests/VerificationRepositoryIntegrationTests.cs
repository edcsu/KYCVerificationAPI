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
}