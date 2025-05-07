using KYCVerificationAPI.Data;
using KYCVerificationAPI.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace KYCVerificationAPI.IntegrationTests;

public class VerificationRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer;
    private AppDbContext _dbContext;
    private VerificationRepository _verificationRepository;
    
    public VerificationRepositoryIntegrationTests()
    {
        _pgContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();
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

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_pgContainer.GetConnectionString())
            .Options;

        _dbContext = new AppDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _verificationRepository = new VerificationRepository(_dbContext);
    }

    public async Task DisposeAsync()
    {
        await _pgContainer.DisposeAsync();
    }
}