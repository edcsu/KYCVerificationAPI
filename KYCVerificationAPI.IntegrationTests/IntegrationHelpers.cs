using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bogus;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Data.Entities;
using KYCVerificationAPI.Features.Vendors.Responses;
using KYCVerificationAPI.Features.Verifications.Requests;
using Microsoft.IdentityModel.Tokens;

namespace KYCVerificationAPI.IntegrationTests;

public static class IntegrationHelpers
{
    
    public static Verification GetVerification(string? userEmail = null)
    {
        var requestFaker = new Faker<Verification>()
            .RuleFor(r => r.CreatedBy, f => string.IsNullOrWhiteSpace(userEmail) ? f.Person.Email : userEmail)
            .RuleFor(r => r.FirstName, f => f.Name.FirstName())
            .RuleFor(r => r.GivenName, f => f.Name.LastName())
            .RuleFor(r => r.Id, f => f.Random.Guid())
            .RuleFor(r => r.Nin, f => f.Random.AlphaNumeric(14))
            .RuleFor(r => r.CardNumber, f => f.Random.AlphaNumeric(9))
            .RuleFor(r => r.KycMessage, f => f.Random.Words(3))
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

    public static string GetToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("SuperSecretToken2025ForYouAreGoingToProsper");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(JwtRegisteredClaimNames.Sub, "test1@uverify.com"),
            new(JwtRegisteredClaimNames.Email, "test1@uverify.com"),
            new("userid", Guid.CreateVersion7().ToString()),
            new("client", "true")
        };
            
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(ApiConstants.TokenLifetime),
            Issuer = "https://auth.uverify.com",
            Audience = "https://kyc.uverify.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var jwt = tokenHandler.WriteToken(token);
        return jwt;
    }
}