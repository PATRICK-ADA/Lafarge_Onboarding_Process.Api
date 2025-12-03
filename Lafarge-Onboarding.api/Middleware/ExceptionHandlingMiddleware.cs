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

            // Set audit status code and status based on response
            context.Items["AuditStatusCode"] = context.Response.StatusCode;
            var auditStatus = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300 ? "Success" : "Failed";
            context.Items["AuditStatus"] = auditStatus;

            // Handle authentication/authorization responses that don't throw exceptions
            if (context.Response.StatusCode == 401 && !context.Response.HasStarted)
            {
                context.Items["AuditStatus"] = "Failed";
                await HandleUnauthorizedAsync(context);
            }
            else if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
            {
                context.Items["AuditStatus"] = "Failed";
                await HandleForbiddenAsync(context);
            }
        }
        catch (Exception ex)
        {
            context.Items["AuditStatus"] = "Failed";
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred. Request: {Method} {Path}, User: {User}", context.Request.Method, context.Request.Path, context.User?.Identity?.Name ?? "Anonymous");

        var statusCode = exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,

            _ => (int)HttpStatusCode.InternalServerError
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
        context.Items["AuditStatusCode"] = statusCode;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private async Task HandleUnauthorizedAsync(HttpContext context)
    {
        var response = new ApiResponse<object>
        {
            Message = "Authentication required. Please provide a valid Bearer token.",
            Result = "Failure!",
            StatusCode = "401",
            IsSuccessful = false,
            TimeStamp = DateTime.UtcNow
        };

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 401;
        context.Items["AuditStatusCode"] = 401;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private async Task HandleForbiddenAsync(HttpContext context)
    {
        var response = new ApiResponse<object>
        {
            Message = "Access forbidden. Insufficient permissions.",
            Result = "Failure!",
            StatusCode = "403",
            IsSuccessful = false,
            TimeStamp = DateTime.UtcNow
        };

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 403;
        context.Items["AuditStatusCode"] = 403;

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}