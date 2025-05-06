namespace KYCVerificationAPI.Data.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default);
    T? GetByIdAsync(Guid id);
    Task<T> Add(T entity, CancellationToken cancellationToken = default);
    Task<T> Update(T entity, CancellationToken cancellationToken = default);
}