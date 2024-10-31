using BuildingBlocks.Exceptions.Handler;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Exceptions.Middleware
{
    public class CustomExceptionMiddleware(RequestDelegate next, CustomExceptionHandler exceptionHandler)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await exceptionHandler.TryHandleAsync(context, ex, context.RequestAborted);
            }
        }
    }
}
