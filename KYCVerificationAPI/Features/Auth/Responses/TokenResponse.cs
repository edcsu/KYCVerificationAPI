using System.ComponentModel;

namespace KYCVerificationAPI.Features.Auth.Responses;

public record TokenResponse
{
    [Description("The access token")]
    public string AccessToken { get; init; } = null!;
    
    [Description("The expiration time in minutes")]
    public double ExpiresIn { get; init; }
}