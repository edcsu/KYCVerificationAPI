using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Verifications.Requests;
using Microsoft.EntityFrameworkCore;

namespace KYCVerificationAPI.Data.Repositories;

public class VerificationRepository : IVerificationRepository
{
    private readonly AppDbContext _context;

    public VerificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Verification>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Verifications.ToListAsync(cancellationToken);
    }

    public async Task<Verification?> GetByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Verifications.FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<Verification> AddAsync(Verification verification, 
        CancellationToken cancellationToken = default)
    {
        await _context.AddAsync(verification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return verification;
    }

    public async Task<Verification> UpdateAsync(Verification verification, 
        CancellationToken cancellationToken = default)
    {
        verification.LastUpdated = DateTime.UtcNow;
        _context.Verifications.Update(verification);
        await _context.SaveChangesAsync(cancellationToken);
        
        return verification;
    }

    public async Task<IEnumerable<Verification>> GetHistoryAsync(VerificationFilter verificationFilter,
        string userEmail, 
        CancellationToken cancellationToken = default)
    {
        var query = _context.Verifications.AsQueryable();

        if (!string.IsNullOrWhiteSpace(verificationFilter.Nin))
            query = query.Where(v => v.Nin.Contains(verificationFilter.Nin));

        var total = await query.CountAsync(cancellationToken);

        var results = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((verificationFilter.Page - 1) * verificationFilter.PageSize)
            .Take(verificationFilter.PageSize)
            .ToListAsync(cancellationToken);
        
        return results;
    }
}