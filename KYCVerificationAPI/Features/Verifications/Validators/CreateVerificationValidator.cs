using FluentValidation;
using KYCVerificationAPI.Features.Verifications.Requests;

namespace KYCVerificationAPI.Features.Verifications.Validators;

public class CreateVerificationValidator:AbstractValidator<CreateVerification>
{
    public CreateVerificationValidator()
    {
        RuleFor(x => x.FirstName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
     
        RuleFor(x => x.GivenName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty();
        
        RuleFor(x => x.CardNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(9);
        
        RuleFor(x => x.Nin)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(14);
    }
}