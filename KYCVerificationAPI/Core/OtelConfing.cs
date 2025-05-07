namespace KYCVerificationAPI.Core;

public class OtelConfing
{
    public const string ConfigName = "OtelConfing";

    /// <summary>
    /// Endpoint to send telemetry
    /// </summary>
    public string Endpoint { get; init; } = null!;

    public bool Enabled { get; init; } = false;
}