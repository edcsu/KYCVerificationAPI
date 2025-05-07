namespace KYCVerificationAPI.Core;

public class RateLimitConfig
{
    public const string SectionName = "RateLimit";

    public int PermitLimit { get; set; } = 1;

    public int Window { get; set; } = 10;
        
    public int ReplenishmentPeriod { get; set; } = 1;
        
    public int QueueLimit { get; set; } = 1;
        
    public int SegmentsPerWindow { get; set; } = 8;
        
    public int TokenLimit { get; set; } = 10;
        
    public int TokenLimit2 { get; set; } = 20;
        
    public int TokensPerPeriod { get; set; } = 4;
        
    public bool AutoReplenishment { get; set; } = false;
}