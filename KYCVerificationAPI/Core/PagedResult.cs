namespace KYCVerificationAPI.Core;

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; init; } = [];
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    
    public bool HasNextPage { get; init; }
    
    public bool HasPreviousPage { get; init; }
}