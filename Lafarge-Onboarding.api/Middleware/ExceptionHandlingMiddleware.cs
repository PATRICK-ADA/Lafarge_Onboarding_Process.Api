using System.Net;
using System.Text.Json;
using Lafarge_Onboarding.domain.OnboardingResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Lafarge_Onboarding.api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            
            // Handle authentication/authorization responses that don't throw exceptions
            if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
            {
                await HandleUnauthorizedAsync(context);
            }
            else if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
            {
                await HandleForbiddenAsync(context);
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var statusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.BadRequest
        };

        var message = exception switch
        {
            ArgumentException => exception.Message,
            InvalidOperationException => exception.Message,
            KeyNotFoundException => "Resource not found",
            UnauthorizedAccessException => "Unauthorized access",
            _ => "An internal error occurred"
        };

        var response = ApiResponse<object>.Failure(message, statusCode.ToString());

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private async Task HandleUnauthorizedAsync(HttpContext context)
    {
        var response = ApiResponse<object>.Failure("Authentication required. Please provide a valid Bearer token.");
        
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 401;
        
        response.StatusCode = "401";
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private async Task HandleForbiddenAsync(HttpContext context)
    {
        var response = ApiResponse<object>.Failure("Access forbidden. Insufficient permissions.");
        
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 403;

        response.StatusCode = "403";
        
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}