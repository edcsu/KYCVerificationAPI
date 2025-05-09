using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using KYCVerificationAPI.Core.Helpers;
using KYCVerificationAPI.Data;
using KYCVerificationAPI.Data.Repositories;
using KYCVerificationAPI.Features.Auth.Requests;
using KYCVerificationAPI.Features.Auth.Validators;
using KYCVerificationAPI.Features.Scheduler.Services;
using KYCVerificationAPI.Features.Vendors.Services;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Service;
using KYCVerificationAPI.Features.Verifications.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using QuestPDF.Infrastructure;
using Serilog;

namespace KYCVerificationAPI.Core.Extensions;

public static class ConfigureServices
{
    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        QuestPDF.Settings.License = LicenseType.Community; 
        
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
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<AuthSecuritySchemeTransformer>();
        });
        
        builder.Services.AddScoped<IVerificationRepository, VerificationRepository>();
        
        builder.Services.AddScoped<IVerificationService, VerificationService>();

        builder.Services.AddScoped<IExternalIdService, ExternalIdService>();
        
        builder.Services.AddScoped<IValidator<CreateVerification>, CreateVerificationValidator>();
        
        builder.Services.AddScoped<IValidator<TokenGenerationRequest>, TokenGenerationRequestValidator>();
        
        builder.Services.AddScoped<IValidator<VerificationFilter>, VerificationFilterValidator>();
        
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
                
                if (!limitOptions.AllowedPaths.Any(allowedPath => path.Contains(allowedPath, StringComparison.OrdinalIgnoreCase)))
                {
                    Log.Information("Rate limiting applied");
                    return RateLimitPartition.GetFixedWindowLimiter(
                        ipAddress,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = limitOptions.PermitLimit,             
                            Window = TimeSpan.FromSeconds(limitOptions.Window), 
                            QueueLimit = limitOptions.QueueLimit,              
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                }

                // Allow unlimited requests for "api" paths
                Log.Information("No rate limiting applied");
                return RateLimitPartition.GetNoLimiter(context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            });
        });
        
        var jwtConfig = config.GetJwtConfig();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(ApiConstants.AdminUserPolicy, p => 
                p.RequireAssertion( a =>
                a.User.HasClaim(c => c is { Type: ApiConstants.AdminUserClaim, Value: "true" })))
            .AddPolicy(ApiConstants.TrustedUserPolicy, p => 
                p.RequireAssertion( a =>
                a.User.HasClaim(c => c is { Type: ApiConstants.AdminUserClaim, Value: "true" }) ||
                a.User.HasClaim(c => c is { Type: ApiConstants.ClientUserClaim, Value: "true" })));
        
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
                ValidIssuer = jwtConfig.Issuer,
                ValidAudience = jwtConfig.Audience,
            };
        });
    }
}