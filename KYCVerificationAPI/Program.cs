using Hangfire;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Extensions;
using KYCVerificationAPI.Core.Filters;
using KYCVerificationAPI.Data;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using SerilogTracing;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateBootstrapLogger();

Log.Information("Starting up Env:{Environment}", environment);
using var listener = new ActivityListenerConfiguration()
    .Instrument.AspNetCoreRequests()
    .TraceToSharedLogger();

try
{
    Log.Information("Starting up"); 
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.AddApiServices();
    
    var app = builder.Build();

    app.UseCors(ApiConstants.AllowedClients);
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "KYC Verification REST API";
            options.ShowSidebar = true;
        });
    }
    
    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
    
        // Attach additional properties to the request completion event
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });
    
    app.AddCorrelationIdMiddleware();
    
    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseApiExceptionHandler();
    
    app.UseAuthorization();
    
    SeedData.Initialize(app);
    
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = [new HangfireAuthorizationFilter()]
    });
    
    app.UseRateLimiter();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    var type = exception.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.OrdinalIgnoreCase))
    {
        throw;
    }

    Log.Fatal(exception, "Unknown exception");
}
finally
{
    Log.Information("Application completely stopped");
    Log.CloseAndFlush();
}