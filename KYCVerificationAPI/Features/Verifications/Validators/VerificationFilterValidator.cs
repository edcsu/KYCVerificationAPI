using FluentValidation;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Features.Vendors.Responses;
using KYCVerificationAPI.Features.Verifications.Requests;

namespace KYCVerificationAPI.Features.Verifications.Validators;

public class VerificationFilterValidator : AbstractValidator<VerificationFilter>
{
    public VerificationFilterValidator()
    {
        RuleFor(v => v.Status)
            .IsEnumName(typeof(VerificationStatus), caseSensitive: false)
            .When(v => !string.IsNullOrWhiteSpace(v.Status));
        
        RuleFor(v => v.KycStatus)
            .IsEnumName(typeof(KycStatus), caseSensitive: false)
            .When(v => !string.IsNullOrWhiteSpace(v.KycStatus));
        
        RuleFor(v => v.FirstName)
            .MaximumLength(100)
            .When(it => !string.IsNullOrWhiteSpace(it.FirstName));
        
        RuleFor(v => v.GivenName)
            .MaximumLength(100)
            .When(it => !string.IsNullOrWhiteSpace(it.GivenName));
        
        RuleFor(v => v.Nin)
            .MaximumLength(14)
            .When(it => !string.IsNullOrWhiteSpace(it.Nin));
        
        RuleFor(v => v.CardNumber)
            .MaximumLength(9)
            .When(it => !string.IsNullOrWhiteSpace(it.CardNumber));
    }
}