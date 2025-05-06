using KYCVerificationAPI.Core.Middleware;

namespace KYCVerificationAPI.Core.Extensions;

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}