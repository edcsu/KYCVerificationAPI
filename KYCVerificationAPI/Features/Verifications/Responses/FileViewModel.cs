namespace KYCVerificationAPI.Features.Verifications.Responses;

public record FileViewModel
{
    public byte[] Contents { get; set; } = [];

    public string ContentType { get; set; } = null!;

    public string? Name { get; set; }
}