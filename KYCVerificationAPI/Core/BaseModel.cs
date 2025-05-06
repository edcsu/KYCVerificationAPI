namespace KYCVerificationAPI.Core;

public class BaseModel
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastUpdated { get; set; }
}