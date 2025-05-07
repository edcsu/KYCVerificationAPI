namespace KYCVerificationAPI.Core;

public static class ApiConstants
{
    public const string AllowedClients = "AllowedClients";
    public const string ApplicationName = "KYCVerificationAPI";
    public const int MaxRetryAttempts = 5;
    public static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);
    public const string AdminUserPolicy = "Admin";
    public const string AdminUserClaim = "admin";
    public const string ClientUserClaim = "client";
    public const string TrustedUserPolicy = "Trusted";

    public static readonly List<string> AllowedPaths =
    [
        "api",
        "scalar",
        "hangfire"
    ];
}