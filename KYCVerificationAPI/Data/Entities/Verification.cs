using System.ComponentModel.DataAnnotations.Schema;
using KYCVerificationAPI.Core;
using KYCVerificationAPI.Features.Vendors.Responses;

namespace KYCVerificationAPI.Data.Entities;

public class Verification : BaseModel
{
    [Column(TypeName = "varchar(100)")]
    public string FirstName { get; init; } = string.Empty;

    [Column(TypeName = "varchar(100)")]
    public string GivenName { get; init; } = string.Empty;
    
    public DateOnly DateOfBirth { get; init; }
    
    [Column(TypeName = "varchar(14)")]
    public string Nin { get; init; } = string.Empty;
    
    [Column(TypeName = "varchar(10)")]
    public string CardNumber { get; init; } = string.Empty;
    
    [Column(TypeName = "varchar(100)")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Column(TypeName = "varchar(50)")]
    public string CorrelationId { get; set; } = string.Empty;
    
    public VerificationStatus Status { get; set; }
    public KycStatus KycStatus { get; set; }

    public int? Retries { get; set; }
    public bool? NameAsPerIdMatches { get; set; }
    public bool? NinAsPerIdMatches { get; set; }
    public bool? CardNumberAsPerIdMatches { get; set; }
    public bool? DateOfBirthMatches { get; set; }
    
    [Column(TypeName = "varchar(100)")]
    public string? KycMessage { get; set; }
}