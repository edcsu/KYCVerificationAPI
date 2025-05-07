using System.ComponentModel.DataAnnotations.Schema;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Features.Vendors.Responses;

namespace KYCVerificationAPI.Data.Entities;

public class Verification : BaseModel
{
    [Column(TypeName = "varchar(25)")]
    public string FirstName { get; init; } = string.Empty;

    [Column(TypeName = "varchar(25)")]
    public string GivenName { get; init; } = string.Empty;
    
    public DateOnly? DateOfBirth { get; init; }
    
    [Column(TypeName = "varchar(14)")]
    public string Nin { get; init; } = string.Empty;
    
    [Column(TypeName = "varchar(10)")]
    public string CardNumber { get; init; } = string.Empty;
    
    [Column(TypeName = "varchar(25)")]
    public string CreatedBy { get; init; } = string.Empty;
    
    [Column(TypeName = "varchar(50)")]
    public string CorrelationId { get; set; } = string.Empty;
    
    public VerificationStatus Status { get; init; }
    public KycStatus KycStatus { get; init; }

    public int? Retries { get; init; }
    public bool? NameAsPerIdMatches { get; init; }
    public bool? NinAsPerIdMatches { get; init; }
    public bool? CardNumberAsPerIdMatches { get; init; }
    public bool? DateOfBirthMatches { get; init; }
}