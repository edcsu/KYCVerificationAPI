using KYCVerificationAPI.Data;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace KYCVerificationAPI.IntegrationTests;

public class SharedFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("shared_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithCleanUp(true)
        .Build();

    private AppDbContext? _dbContext;
    public string DatabaseConnectionString => _dbContainer.GetConnectionString();
    public AppDbContext DbContext => _dbContext;
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(DatabaseConnectionString)
            .Options;
        _dbContext = new AppDbContext(optionsBuilder);

        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        // Clear tables between tests for isolation
        DbContext.Verifications.RemoveRange(DbContext.Verifications);
        await DbContext.SaveChangesAsync();
    }
}