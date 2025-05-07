using KYCVerificationAPI.Core.Middleware;

namespace KYCVerificationAPI.Core.Extensions;

public static class CorrelationIdMiddleWareExtensions
{
    public static IApplicationBuilder AddCorrelationIdMiddleware(this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseMiddleware<CorrelationIdMiddleware>();
}