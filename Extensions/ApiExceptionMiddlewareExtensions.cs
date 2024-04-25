using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CatalogAPI.Models;

public static class ApiExceptionMiddlewareExtensions
{
    // Método de extensão
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (contextFeature != null)
            {
                await context.Response.WriteAsync(new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = contextFeature.Error.Message,
                    Trace = contextFeature.Error.StackTrace
                }.ToString());
            }
        });
        });
    }
}