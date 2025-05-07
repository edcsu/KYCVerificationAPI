using Hangfire.Dashboard;

namespace KYCVerificationAPI.Core.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        return true;
        // Allow all authenticated users to see the Dashboard.
        return httpContext.User.Identity?.IsAuthenticated ?? false;
    }
}