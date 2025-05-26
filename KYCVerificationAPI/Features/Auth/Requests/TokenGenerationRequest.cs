using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KYCVerificationAPI.Features.Auth.Requests;

public record TokenGenerationRequest
{
    [Description("The unique identifier for the user")]
    public required Guid UserId { get; set; }
    
    [Description("The email address of the user")]
    [EmailAddress]
    public required string Email { get; set; }

    [Description("A collection of claims for the user")]
    [MinLength(1)]
    public required Dictionary<string, object> CustomClaims { get; init; } = new();
}