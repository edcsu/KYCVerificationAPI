using System.ComponentModel;

namespace KYCVerificationAPI.Features.Verifications.Requests;

public class VerificationFilter
{
    private int _pageSize = 10;
    private const int MaxPageSize = 50;

    [Description("Unique identifier of the verification")]
    public Guid? TransactionId { get; set; }
    
    [Description("Status of the Verification")]
    public string? Status { get; set; }
    
    [Description("KYC status of the Verification")]
    public string? KycStatus { get; init; }
    
    [Description("Match result of the names")]
    public bool? NameAsPerIdMatches { get; set; }
    
    [Description("Match result of the NIN")]
    public bool? NinAsPerIdMatches { get; set; }
    
    [Description("Match result of the card number")]
    public bool? CardNumberAsPerIdMatches { get; set; }
    
    [Description("Match result of the date of birth")]
    public bool? DateOfBirthMatches { get; set; }
    
    [Description("First name of the holder on their National ID card")]
    public string? FirstName { get; init; }
    
    [Description("Given name of the holder on their National ID card")]
    public string? GivenName { get; init; }
    
    [Description("Card number of the holder on their National ID card")]
    public string? CardNumber { get; init; }
    
    [Description("NIN of the holder on their National ID card")]
    public string? Nin { get; init; }
    
    [Description("Date of birth of the holder on their National ID card")]
    public DateOnly? DateOfBirth { get; init; }
    
    [Description("The start date of the date range")]
    public DateOnly? From { get; init; }
    
    [Description("The end date of the date range")]
    public DateOnly? To { get; init; }
    
    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

}