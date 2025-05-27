using KYCVerificationAPI.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KYCVerificationAPI.IntegrationTests;

public class KycWebApplicationFactory(SharedFixture sharedFixture) : WebApplicationFactory<Program>, IAsyncLifetime
{
    public SharedFixture SharedFixture => sharedFixture;
    
    public HttpClient HttpClient { get; private set; } = null!;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", sharedFixture.DatabaseConnectionString);

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
                options.UseNpgsql(sharedFixture.DatabaseConnectionString, 
                    opts =>
                    {
                        opts.EnableRetryOnFailure();
                    });
            });
        });
    }

    public Task InitializeAsync()
    {
        HttpClient = CreateClient();
        return Task.CompletedTask;
    }

    public new Task DisposeAsync() => Task.CompletedTask;
}