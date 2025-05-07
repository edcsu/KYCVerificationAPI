namespace KYCVerificationAPI.Core;

public class RateLimitConfig
{
    public const string SectionName = "RateLimit";

    public int PermitLimit { get; set; } = 1;

    public int Window { get; set; } = 5;
        
    public int QueueLimit { get; set; } = 15;
}