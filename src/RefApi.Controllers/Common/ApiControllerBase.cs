using Microsoft.AspNetCore.Mvc;

namespace RefApi.Controllers.Common;

[ApiController]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Creates an <see cref="OkObjectResult"/> if <paramref name="value"/> is not null,
    /// otherwise returns a <see cref="NotFoundResult"/>
    /// </summary>
    /// <typeparam name="T">The type of the value to return in the HTTP 200 response.</typeparam>
    /// <param name="value">The value to include in the response body if not null.</param>
    /// <returns></returns>
    protected ActionResult OkOrNotFound<T>(T? value)
    {
        return value is null ? NotFound() : Ok(value);
    }

    /// <summary>
    /// Creates an <see cref="OkResult"/> if the specified <paramref name="condition"/> is true,
    /// otherwise returns a <see cref="NotFoundResult"/>.
    /// </summary>
    /// <param name="condition">A boolean indicating whether to return an <see cref="OkResult"/> or a <see cref="NotFoundResult"/>.</param>
    /// <returns>An <see cref="OkResult"/> if the condition is true, otherwise a <see cref="NotFoundResult"/>.</returns>
    protected ActionResult OkOrNotFound(bool condition)
    {
        return condition ? Ok() : NotFound();
    }
}