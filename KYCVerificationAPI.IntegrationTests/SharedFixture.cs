using DotNet.Testcontainers.Builders;
using KYCVerificationAPI.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace KYCVerificationAPI.IntegrationTests;

public class SharedFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("shared_db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
        .WithCleanUp(true)
        .Build();

    private AppDbContext? _dbContext;
    private Respawner _respawner = null!;
    
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
        await InitializeRespawnerAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var conn = new NpgsqlConnection(DatabaseConnectionString);
        await conn.OpenAsync();

        await _respawner.ResetAsync(conn);
    }
    
    private async Task InitializeRespawnerAsync()
    {
        await using var conn = new NpgsqlConnection(DatabaseConnectionString);
        await conn.OpenAsync();

        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions{
            SchemasToInclude =
            [
                "public",
                "postgres"
            ], 
            DbAdapter = DbAdapter.Postgres 
        });
    }
}