namespace KYCVerificationAPI.Core.Extensions;

public static class ConfigExtensions
{
    /// <summary>
    /// Get Open Telemetry settings
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static OtelConfing GetOtelConfing(this IConfiguration configuration)
    {
        return configuration.GetSection(OtelConfing.ConfigName).Get<OtelConfing>();
    }
    
    public static JwtConfig GetJwtConfig(this IConfiguration configuration)
    {
        return configuration.GetSection(JwtConfig.ConfigName).Get<JwtConfig>();
    }
}