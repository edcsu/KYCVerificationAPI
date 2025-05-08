namespace KYCVerificationAPI.Features.Verifications.Requests;

public class VerificationFilter
{
    private int _pageSize = 10;
    private const int MaxPageSize = 50;

    public string? Status { get; set; }
    public bool? IsMatch { get; set; }
    public string? Nin { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

}