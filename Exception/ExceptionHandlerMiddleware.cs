using System.Net;

namespace Social_Setting.Exception;

public class ExceptionHandlerMiddleware: IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (System.Exception e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await context.Response.WriteAsync(e.Message);
        }
    }
}