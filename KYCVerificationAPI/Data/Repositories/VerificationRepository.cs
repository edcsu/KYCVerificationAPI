using KYCVerificationAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace KYCVerificationAPI.Data.Repositories;

public class VerificationRepository : IVerificationRepository
{
    private readonly AppDbContext _context;

    public VerificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Verification>> GetAll(CancellationToken cancellationToken = default)
    {
        return await _context.Verifications.ToListAsync(cancellationToken);
    }

    public async Task<Verification?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Verifications.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Verification> Add(Verification verification, 
        CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(verification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return verification;
    }

    public async Task<Verification> Update(Verification verification, CancellationToken cancellationToken = default)
    {
        verification.LastUpdated = DateTime.UtcNow;
        _context.Verifications.Update(verification);
        await _context.SaveChangesAsync(cancellationToken);
        
        return verification;
    }
}