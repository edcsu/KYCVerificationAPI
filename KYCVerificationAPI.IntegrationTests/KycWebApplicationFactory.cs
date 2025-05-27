using DotNet.Testcontainers.Builders;
using KYCVerificationAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace KYCVerificationAPI.IntegrationTests;

public class KycWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase($"kyc_test_db_{Guid.NewGuid()}")  // Unique DB name per instance
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithWaitStrategy(Wait.ForUnixContainer())
        .WithCleanUp(true)
        .Build();

    public AppDbContext DbContext { get; private set; } = null!;
    public HttpClient HttpClient { get; private set; } = null!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // builder.ConfigureAppConfiguration((context, configBuilder) =>
        // {
        //     // Clear any existing configuration sources
        //     configBuilder.Sources.Clear();
        //     
        //     // Add minimal configuration needed for tests
        //     configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        //     {
        //         {"ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString()}
        //     });
        // });
        builder.UseSetting("ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString());


        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString(), 
                    opts =>
                    {
                        opts.EnableRetryOnFailure();
                    });
            });
        });
    }


    public async Task InitializeAsync()
    {
        try
        {
            await _dbContainer.StartAsync();
            
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(_dbContainer.GetConnectionString())
                .Options;

            DbContext = new AppDbContext(options);
            
            // Create the HTTP client
            HttpClient = CreateClient();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize the test environment", ex);
        }
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}