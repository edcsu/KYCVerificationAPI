namespace KYCVerificationAPI.Core;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}