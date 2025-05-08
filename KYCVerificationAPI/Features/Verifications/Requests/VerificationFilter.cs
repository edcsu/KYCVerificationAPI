namespace KYCVerificationAPI.Features.Verifications.Requests;

public class VerificationFilter
{
    private int _pageSize = 10;
    private const int MaxPageSize = 50;

    public Guid? TransactionId { get; set; }
    
    public string? Status { get; set; }
    
    public bool? NameAsPerIdMatches { get; set; }
    
    public bool? NinAsPerIdMatches { get; set; }
    
    public bool? CardNumberAsPerIdMatches { get; set; }
    
    public bool? DateOfBirthMatches { get; set; }
    
    public string? FirstName { get; init; }
    
    public string? GivenName { get; init; }
    
    public string? CardNumber { get; init; }
    public string? Nin { get; init; }
    
    public DateOnly? DateOfBirth { get; init; }
    
    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

}