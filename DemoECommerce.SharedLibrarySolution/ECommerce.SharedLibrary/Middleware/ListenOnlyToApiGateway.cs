using Microsoft.AspNetCore.Http;

namespace ECommerce.SharedLibrary.Middleware
{
    public class ListenOnlyToApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var signedHeader = context.Response.Headers["Api-Gateway"];

            if (signedHeader.FirstOrDefault() is null) 
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service is unavailable");
                return;
            }
            else
            {
                await next(context);
            }
        }
    }
}
