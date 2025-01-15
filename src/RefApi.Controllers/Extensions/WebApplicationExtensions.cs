using Microsoft.AspNetCore.Diagnostics;

using RefApi.Controllers.Common.Exceptions;

namespace RefApi.Controllers.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler("/error");

        app.Map("error", (HttpContext httpContext) =>
        {
            Exception? exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception is null)
            {
                return Results.Problem();
            }

            return exception switch
            {
                ProblemException problemException => Results.Problem(
                    statusCode: 400,
                    detail: problemException.ErrorMessage),
                _ => Results.Problem()
            };
        });

        return app;
    }
}