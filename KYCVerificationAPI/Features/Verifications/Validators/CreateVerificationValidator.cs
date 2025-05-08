using FluentValidation;
using KYCVerificationAPI.Features.Verifications.Requests;

namespace KYCVerificationAPI.Features.Verifications.Validators;

public class CreateVerificationValidator:AbstractValidator<CreateVerification>
{
    public CreateVerificationValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
     
        RuleFor(x => x.GivenName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty();
        
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Length(9);
        
        RuleFor(x => x.Nin)
            .NotEmpty()
            .Length(14);
    }
}