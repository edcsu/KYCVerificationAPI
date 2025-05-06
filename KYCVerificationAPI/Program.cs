using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Extensions;
using Serilog;
using Serilog.Events;
using SerilogTracing;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .Enrich.WithProperty("Application", ApiConstants.ApplicationName)
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Trace()
    .WriteTo.File("Logs/applog.log", rollingInterval: RollingInterval.Hour)
    .CreateLogger();

using var listener = new ActivityListenerConfiguration()
    .Instrument.AspNetCoreRequests()
    .TraceToSharedLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.AddApiServices();
    
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.UseCors(ApiConstants.AllowedClients);
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    
    app.UseSerilogRequestLogging(options =>
    {
        // Emit debug-level events instead of the defaults
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;
    
        // Attach additional properties to the request completion event
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? "unknown");
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });
    
    app.UseHttpsRedirection();

    app.UseApiExceptionHandler();

    app.UseAuthorization();

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