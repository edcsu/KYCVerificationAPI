using System.ComponentModel;
using KYCVerificationAPI.Core;

namespace KYCVerificationAPI.Features.Vendors.Responses;

public record KycResponse
{
    [Description("Status of the request")]
    public KycStatus KycStatus { get; init; }
    
    [Description("Description of the verification result")]
    public string? Message { get; init; }
    
    [Description("Match result of the names")]
    public bool? NameAsPerIdMatches { get; set; }
    
    [Description("Match result of the nin")]
    public bool? NinAsPerIdMatches { get; set; }
    
    [Description("Match result of the card number")]
    public bool? CardNumberAsPerIdMatches { get; set; }
    
    [Description("Match result of the date of birth")]
    public bool? DateOfBirthMatches { get; set; }
}