using KYCVerificationAPI.Data.Entities;

namespace KYCVerificationAPI.Data.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken = default);
    Task<Verification?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<T> Add(T entity, CancellationToken cancellationToken = default);
    Task<T> Update(T entity, CancellationToken cancellationToken = default);
}