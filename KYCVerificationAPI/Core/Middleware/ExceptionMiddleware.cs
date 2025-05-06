using System.Net.Mime;
using System.Text.Json;
using KYCVerificationAPI.Core.Exceptions;
using Serilog;

namespace KYCVerificationAPI.Core.Middleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)

        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        Log.Error("{GlobalException}", ex);
        var message = "Oops something went wrong!";
        switch (ex)
        {
            case ClientSideException _:
            
            case ModelStateException _:
                context.Response.StatusCode = 400;
                message = ex.Message;
                break;

            default:
                context.Response.StatusCode = 500;
                break;
        }

        context.Response.ContentType = MediaTypeNames.Application.Json;
        var result = JsonSerializer.Serialize(new
        {
            message = message
        });
        return context.Response.WriteAsync(result);
    }
}