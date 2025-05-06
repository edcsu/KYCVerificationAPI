namespace KYCVerificationAPI.Core;

public static class ConfigureServices
{
    public static void AddApiServices(this WebApplicationBuilder builder)
    {
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
    }
}