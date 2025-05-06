using System.ComponentModel;
using KYCVerificationAPI.Core;

namespace KYCVerificationAPI.Features.Verifications.Responses;

public record VerificationData
{
    [Description("FirstName of the holder on their National ID card")]
    public string FirstName { get; set; } = string.Empty;

    [Description("GivenName of the holder on their National ID card")]
    public string GivenName { get; init; } = string.Empty;
    
    [Description("DateOfBirth of the holder on their National ID card")]

    public DateOnly DateOfBirth { get; set; } 
    
    [Description("Nin of the holder on their National ID card")]

    public string Nin { get; init; } = string.Empty;
    
    [Description("CardNumber of the holder on their National ID card")]
    public string CardNumber { get; init; } = string.Empty;
    
    [Description("Status of the Verification")]
    public VerificationStatus Status { get; init; }
    
    [Description("Match result of the names")]
    public bool NameAsPerIdMatches { get; set; }
    
    [Description("Match result of the nin")]
    public bool NinAsPerIdMatches { get; set; }
    
    [Description("Match result of the card number")]
    public bool CardNumberAsPerIdMatches { get; set; }
    
    [Description("Match result of the date of birth")]
    public bool DateOfBirthMatches { get; set; }
}