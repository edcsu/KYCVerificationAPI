using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KYCVerificationAPI.Features.Verifications.Requests;

public record CreateVerification{
    [Description("FirstName of the holder on their National ID card")]
    [Length(2,25)]
    public required string FirstName { get; init; }
    
    [Description("GivenName of the holder on their National ID card")]
    [Length(2,25)]
    public required string GivenName { get; init; }
    
    [Description("DateOfBirth of the holder on their National ID card")]
    public required DateOnly DateOfBirth { get; init; }
    
    [Description("Nin of the holder on their National ID card")]
    [Length(14,14)]
    public required string Nin { get; init; }

    [Description("CardNumber of the holder on their National ID card")]
    [Length(9,9)]
    public required string CardNumber { get; init; }
}