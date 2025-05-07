using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Data;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Scheduler.Services;
using KYCVerificationAPI.Features.Vendors.Services;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Service;
using KYCVerificationAPI.Features.Verifications.Validators;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace KYCVerificationAPI.Core.Extensions;

public static class ConfigureServices
{
    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString("DefaultConnection"), 
                opts =>
            {
                opts.EnableRetryOnFailure();
            });
        });
        
        builder.Services.AddHangfire(configuration =>
            configuration.
                SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(c =>
                    c.UseNpgsqlConnection(config.GetConnectionString("DefaultConnection")))
        );
            
        builder.Services.AddHangfireServer();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(ApiConstants.AllowedClients, policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        
        builder.AddCustomApiVersioning();
        
        builder.AddSerilogConfig();
        
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.WriteIndented = true;

            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        builder.Services.AddScoped<IVerificationRepository, VerificationRepository>();
        
        builder.Services.AddScoped<IVerificationService, VerificationService>();

        builder.Services.AddScoped<IExternalIdService, ExternalIdService>();
        
        builder.Services.AddScoped<IValidator<CreateVerification>, CreateVerificationValidator>();
        
        builder.Services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
        
        builder.Services.AddScoped<ISchedulerService, SchedulerService>();
        
        var otelConfig = config.GetOtelConfing();

        if (otelConfig.Enabled)
        {
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(ApiConstants.ApplicationName))
                .WithMetrics(metric =>
                {
                    metric.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    metric.AddOtlpExporter(options => 
                    {
                        options.Endpoint = new Uri(otelConfig.Endpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
                })
                .WithTracing(trace =>
                {
                    trace.AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    trace.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otelConfig.Endpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
                });
                
            builder.Logging.AddOpenTelemetry(options => 
            {
                if (environment == Environments.Development)
                {
                    options.AddConsoleExporter()
                        .SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService(ApiConstants.ApplicationName));
                }

                options.AddOtlpExporter(otlpExporterOptions =>
                {
                    otlpExporterOptions.Endpoint = new Uri(otelConfig.Endpoint);
                    otlpExporterOptions.Protocol = OtlpExportProtocol.Grpc;
                });
            });
        }
        
        var limitOptions = new RateLimitConfig();
        builder.Configuration.GetSection(RateLimitConfig.SectionName).Bind(limitOptions);
        
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var path = context.Request.Path.Value ?? string.Empty;
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                Log.Information("This request is coming from: {IpAddress}", ipAddress);

                // if (!path.Contains("api", StringComparison.OrdinalIgnoreCase) ||
                //     !path.Contains("scalar", StringComparison.OrdinalIgnoreCase) ||
                //     !path.Contains("hangfire", StringComparison.OrdinalIgnoreCase))
                // {
                //     Log.Information("Rate limiting applied");
                //     return RateLimitPartition.GetFixedWindowLimiter(
                //         ipAddress,
                //         partition => new FixedWindowRateLimiterOptions
                //         {
                //             PermitLimit = limitOptions.PermitLimit,             // 1 requests
                //             Window = TimeSpan.FromMinutes(limitOptions.Window), // Per 5 minutes
                //             QueueLimit = limitOptions.QueueLimit,              // Allow 5 requests in the queue
                //             QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                //         });
                // }

                // Allow unlimited requests for "api" paths
                Log.Information("No rate limiting applied");
                return RateLimitPartition.GetNoLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            });
        });
    }
}