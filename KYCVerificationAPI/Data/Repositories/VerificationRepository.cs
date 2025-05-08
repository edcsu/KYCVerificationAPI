using KYCVerificationAPI.Core;
using KYCVerificationAPI.Core.Extensions;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Verifications.Mappings;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;
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

    public async Task<PagedResult<VerificationResponse>> GetHistoryAsync(VerificationFilter verificationFilter,
        string userEmail,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Verifications.AsQueryable();
        
        query = query.Where(v => v.CreatedBy == userEmail);

        if (!string.IsNullOrWhiteSpace(verificationFilter.Nin))
            query = query.Where(v => v.Nin.Contains(verificationFilter.Nin));
        
        if (!string.IsNullOrWhiteSpace(verificationFilter.CardNumber))
            query = query.Where(v => v.CardNumber.Contains(verificationFilter.CardNumber));
        
        if (!string.IsNullOrWhiteSpace(verificationFilter.FirstName))
            query = query.Where(v => v.FirstName.Contains(verificationFilter.FirstName));
        
        if (!string.IsNullOrWhiteSpace(verificationFilter.GivenName))
            query = query.Where(v => v.GivenName.Contains(verificationFilter.GivenName));
        
        if (verificationFilter.NameAsPerIdMatches.HasValue)
            query = query.Where(v => v.NameAsPerIdMatches == verificationFilter.NameAsPerIdMatches);
        
        if (verificationFilter.NinAsPerIdMatches.HasValue)
            query = query.Where(v => v.NinAsPerIdMatches == verificationFilter.NinAsPerIdMatches);
        
        if (verificationFilter.CardNumberAsPerIdMatches.HasValue)
            query = query.Where(v => v.CardNumberAsPerIdMatches == verificationFilter.CardNumberAsPerIdMatches);
        
        if (verificationFilter.DateOfBirthMatches.HasValue)
            query = query.Where(v => v.NameAsPerIdMatches == verificationFilter.DateOfBirthMatches);

        var total = await query.CountAsync(cancellationToken);

        var verificationResponses = await query
            .OrderByDescending(v => v.CreatedAt)
            .ApplyPaging(verificationFilter.Page, verificationFilter.PageSize)
            .Select(it => it.MapToVerificationResponse())
            .ToListAsync(cancellationToken);
        
        return new PagedResult<VerificationResponse>
        {
            Items = verificationResponses,
            TotalItems = total,
            Page = verificationFilter.Page,
            PageSize = verificationFilter.PageSize
        };
    }
}