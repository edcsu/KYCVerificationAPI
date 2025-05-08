using KYCVerificationAPI.Data.Repositories;

namespace KYCVerificationAPI.IntegrationTests;

public class VerificationRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly VerificationRepository _verificationRepository;
    
    public VerificationRepositoryTests(DatabaseFixture fixture)
    {
        _verificationRepository = new VerificationRepository(fixture.DbContext);
    }
    
    [Fact]
    public async Task Add_ShouldAddNewVerification()
    {
        // Arrange
        var newVerification = IntegrationHelpers.GetVerification();
        
        // Act
        var result = await _verificationRepository.AddAsync(newVerification);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newVerification, result);
    }
    
    [Fact]
    public async Task Update_ShouldUpdateExistingVerification()
    {
        // Arrange
        var newVerification = IntegrationHelpers.GetVerification();
        var savedVerification = await _verificationRepository.AddAsync(newVerification);

        const string newMessage = "The KYC is valid";
        savedVerification.KycMessage = newMessage;
        // Act
        var result = await _verificationRepository.UpdateAsync(savedVerification);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newMessage, result.KycMessage);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnVerification_WhenVerificationExists()
    {
        // Arrange
        var expectedVerification = IntegrationHelpers.GetVerification();
        await _verificationRepository.AddAsync(expectedVerification);

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
        await _verificationRepository.AddAsync(newVerification);

        // Act
        var result = await _verificationRepository.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single( result);
    }
    
    [Fact]
    public async Task GetById_ShouldReturnNull_WhenVerificationDoesNotExist()
    {
        // Arrange
        var expectedId = Guid.NewGuid();

        // Act
        var result = await _verificationRepository.GetByIdAsync(expectedId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}