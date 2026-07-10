using Mimre.Domain.Exceptions;
using Mimre.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Mimre.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/openapi"))
        {
            await next(context);
            return;
        }

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception on {Method} {Path}. {ExceptionType}: {ExceptionMessage}",
                context.Request.Method,
                context.Request.Path,
                ex.GetType().Name,
                ex.Message);
            await HandleExceptionAsync(context, ex, environment.IsDevelopment());
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, bool isDevelopment)
    {
        var (statusCode, title, errors) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                ve.Errors),

            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                nfe.Message,
                (IReadOnlyDictionary<string, string[]>?)null),

            DomainException de => (
                HttpStatusCode.BadRequest,
                de.Message,
                (IReadOnlyDictionary<string, string[]>?)null),

            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                (IReadOnlyDictionary<string, string[]>?)null)
        };

        var response = isDevelopment ?
            (object)new
            {
                title,
                status = (int)statusCode,
                errors,
                detail = exception.ToString()
            } :
            new
            {
                title,
                status = (int)statusCode,
                errors
            };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
