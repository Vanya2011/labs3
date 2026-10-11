using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.WebApi.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
        {
            if (exception is ValidationException validationException)
            {
                _logger.LogWarning("Validation failed: {Message}. Errors count: {Count}",
                    validationException.Message,
                    validationException.Errors.Count());

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                var errors = validationException.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );

                var problemDetails = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Request validation error",
                    Detail = "One or more parameters failed validation."
                };

                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                return true;
            }

            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var genericProblem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = exception.Message
            };

            await httpContext.Response.WriteAsJsonAsync(genericProblem, cancellationToken);
            return true;
        }

    }
}
