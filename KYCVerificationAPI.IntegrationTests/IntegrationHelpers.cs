using Bogus;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Vendors.Responses;

namespace KYCVerificationAPI.IntegrationTests;

public static class IntegrationHelpers
{
    
    public static Verification GetVerification()
    {
        var requestFaker = new Faker<Verification>()
            .RuleFor(r => r.CreatedBy, f => f.Person.Email)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.GivenName, f => f.Name.LastName())
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.Nin, f => f.Random.AlphaNumeric(14))
            .RuleFor(r => r.CardNumber, f => f.Random.AlphaNumeric(9))
            .RuleFor(r => r.KycMessage, f => f.Random.AlphaNumeric(10))
            .RuleFor(r => r.KycStatus, f => f.PickRandom<KycStatus>())
            .RuleFor(r => r.Status, f => f.PickRandom<VerificationStatus>())
            .RuleFor(r => r.CorrelationId, f => f.Random.Guid().ToString())
            .RuleFor(r => r.DateOfBirth, f => f.Date.PastDateOnly())
            .RuleFor(r => r.NameAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.NinAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.CardNumberAsPerIdMatches, f => f.Random.Bool())
            .RuleFor(r => r.DateOfBirthMatches, f => f.Random.Bool())
            .RuleFor(r => r.LastUpdated, f => f.Date.Past().ToUniversalTime());
        return requestFaker.Generate();
    }
}