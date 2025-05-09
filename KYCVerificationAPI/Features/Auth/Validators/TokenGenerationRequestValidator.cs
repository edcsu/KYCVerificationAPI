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
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress();
        
        RuleFor(request => request.CustomClaims)
            .NotEmpty()
            .Must(claims => claims.ContainsKey(ApiConstants.ClientUserClaim) || 
                            claims.ContainsKey(ApiConstants.AdminUserClaim))
            .WithMessage("CustomClaims must contain a 'client' key");
    }
    
}