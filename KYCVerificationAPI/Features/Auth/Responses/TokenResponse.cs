using System.ComponentModel;

namespace KYCVerificationAPI.Features.Auth.Responses;

public record TokenResponse
{
    [Description("The access token")]
    public string AccessToken { get; set; } = null!;
    
    [Description("The time in minutes when the token expires")]
    public double ExpiresIn { get; set; }
}