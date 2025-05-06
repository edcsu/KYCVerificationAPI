using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();

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