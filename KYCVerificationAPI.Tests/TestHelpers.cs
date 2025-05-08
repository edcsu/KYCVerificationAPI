using Bogus;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Vendors.Responses;
using KYCVerificationAPI.Features.Verifications.Mappings;
using KYCVerificationAPI.Features.Verifications.Requests;
using KYCVerificationAPI.Features.Verifications.Responses;

namespace KYCVerificationAPI.Tests;

public static class TestHelpers
{
    public static CreateVerification GetCreateVerification()
    {
        var requestFaker = new Faker<CreateVerification>()
            .RuleFor(r => r.Nin, f => f.Random.AlphaNumeric(14))
            .RuleFor(r => r.CardNumber, f => f.Random.AlphaNumeric(9))
            .RuleFor(r => r.DateOfBirth, f => f.Date.PastDateOnly())
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.GivenName, f => f.Name.LastName());
        return requestFaker.Generate();
    }
        
    public static List<Verification> GetVerifications(int max = 10)
    {
        var requestFaker = new Faker<Verification>()
            .RuleFor(r => r.CreatedBy, f => f.Person.Email)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.GivenName, f => f.Name.LastName())
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.Nin, f => f.Random.AlphaNumeric(14))
            .RuleFor(r => r.CardNumber, f => f.Random.AlphaNumeric(9))
            .RuleFor(r => r.KycMessage, f => f.Random.Words(5))
            .RuleFor(r => r.KycStatus, f => f.PickRandom<KycStatus>())
            .RuleFor(r => r.Status, f => f.PickRandom<VerificationStatus>())
            .RuleFor(r => r.CorrelationId, f => f.Random.Guid().ToString())
            .RuleFor(r => r.DateOfBirth, f => f.Date.PastDateOnly())
            .RuleFor(r => r.NameAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.NinAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.CardNumberAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.DateOfBirthMatches, f => f.Random.Bool())
            .RuleFor(r => r.CreatedAt, f => f.Date.Past().ToUniversalTime())
            .RuleFor(r => r.LastUpdated, f => f.Date.Past().ToUniversalTime());
        return requestFaker.Generate(max);
    }
    
    public static PagedResult<VerificationResponse> GetPagedResponse(VerificationFilter filter, string userEmail,
        int max = 20)
    {
        var requestFaker = new Faker<Verification>()
            .RuleFor(r => r.CreatedBy, f => userEmail)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.GivenName, f => f.Name.LastName())
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.Nin, f => f.Random.AlphaNumeric(14))
            .RuleFor(r => r.CardNumber, f => f.Random.AlphaNumeric(9))
            .RuleFor(r => r.KycMessage, f => f.Random.Words(5))
            .RuleFor(r => r.KycStatus, f => f.PickRandom<KycStatus>())
            .RuleFor(r => r.Status, f => f.PickRandom<VerificationStatus>())
            .RuleFor(r => r.CorrelationId, f => f.Random.Guid().ToString())
            .RuleFor(r => r.DateOfBirth, f => f.Date.PastDateOnly())
            .RuleFor(r => r.NameAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.NinAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.CardNumberAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.DateOfBirthMatches, f => f.Random.Bool())
            .RuleFor(r => r.CreatedAt, f => f.Date.Past().ToUniversalTime())
            .RuleFor(r => r.LastUpdated, f => f.Date.Past().ToUniversalTime());
        var requests = requestFaker.Generate(max);
        var totalPages = (int)Math.Ceiling(requests.Count / (double)filter.PageSize);
        
        var verificationResponses = requests.Select(it => it.MapToVerificationResponse());
        return new PagedResult<VerificationResponse>
        {
            Data = verificationResponses,
            TotalItems = requests.Count,
            Page = filter.Page,
            PageSize = filter.PageSize,
            HasNextPage = filter.Page < totalPages,
            HasPreviousPage = filter.Page > 1,
        };
    }
}