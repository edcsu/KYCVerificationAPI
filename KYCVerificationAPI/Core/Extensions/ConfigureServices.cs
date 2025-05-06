using System.Text.Json.Serialization;
using FluentValidation;
using KYCVerificationAPI.Data;
using KYCVerificationAPI.Features.Vendors.Services;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Service;
using KYCVerificationAPI.Features.Verifications.Validators;
using Microsoft.EntityFrameworkCore;

namespace KYCVerificationAPI.Core.Extensions;

public static class ConfigureServices
{
    public static void AddApiServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), opts =>
            {
                opts.EnableRetryOnFailure();
            });
        });
        
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
        
        builder.Services.AddScoped<IVerificationService, VerificationService>();

        builder.Services.AddScoped<IExternalIdService, ExternalIdService>();
        
        builder.Services.AddScoped<IValidator<CreateVerification>, CreateVerificationValidator>();

    }
}