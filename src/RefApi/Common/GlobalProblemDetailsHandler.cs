using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RefApi.Common;

public class GlobalProblemDetailsHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            FluentValidation.ValidationException fluentEx => HandleValidationException(fluentEx),
            ArgumentException argEx => HandleArgumentException(argEx),

            _ => null
        };

        if (problemDetails is null)
        {
            return false;
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static ProblemDetails HandleValidationException(FluentValidation.ValidationException ex)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        var errors = ex.Errors.Select(error => error.ErrorMessage).ToList();
        problemDetails.Extensions.Add("errors", errors);

        return problemDetails;
    }

    private static ProblemDetails HandleArgumentException(ArgumentException ex)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid argument",
            Detail = ex.Message,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };
    }
}