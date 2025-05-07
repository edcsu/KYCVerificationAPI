using FluentValidation;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Features.Auth.Requests;

namespace KYCVerificationAPI.Features.Auth.Validators;

public class TokenGenerationRequestValidator: AbstractValidator<TokenGenerationRequest>
{
    public TokenGenerationRequestValidator()
    {
        RuleFor(request => request.UserId).NotEmpty();
        
        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(request => request.CustomClaims)
            .NotEmpty()
            .Must(claims => claims.ContainsKey(ApiConstants.ClientUserClaim))
            .WithMessage("CustomClaims must contain a 'client' key");
    }
    
}