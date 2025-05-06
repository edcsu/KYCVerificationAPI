using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;

namespace KYCVerificationAPI.Core.Extensions;

public static class SerilogConfig
{
    /// <summary>
    /// Add Serilog config with enrichers
    /// </summary>
    /// <param name="builder"></param>
    public static void AddSerilogConfig(this WebApplicationBuilder builder)
    {

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((ctx, services, lc) =>
        {
            lc
                .ReadFrom.Configuration(ctx.Configuration)
                .ReadFrom.Services(services)
                .Enrich.WithThreadId()
                .Enrich.WithMachineName()
                .Enrich.WithThreadName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithClientIp()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithExceptionDetails(
                    new DestructuringOptionsBuilder()
                        .WithDefaultDestructurers());
        });
    }
}