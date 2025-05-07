namespace KYCVerificationAPI.Core;

public static class ApiConstants
{
    public const string AllowedClients = "AllowedClients";
    public const string ApplicationName = "KYCVerificationAPI";
    public const int MaxRetryAttempts = 5;
    public const string TokenSecret = "SuperSecretToken2025ForYouAreGoingToProsper";
    public static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(1);
}