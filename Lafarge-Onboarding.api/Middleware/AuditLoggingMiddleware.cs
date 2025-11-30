namespace Lafarge_Onboarding.api.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;

    public AuditLoggingMiddleware(
        RequestDelegate next,
        ILogger<AuditLoggingMiddleware> logger,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;

        var requestDetails = new
        {
            Method = context.Request.Method,
            Path = context.Request.Path.ToString(),
            QueryString = context.Request.QueryString.ToString(),
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            ContentType = context.Request.ContentType,
            ContentLength = context.Request.ContentLength
        };

        var userId = context.User?.FindFirst("sub")?.Value ??
                    context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userName = context.User?.Identity?.Name;
        var userEmail = context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
        {
            ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??
                       context.Request.Headers["X-Real-IP"].FirstOrDefault();
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();

            try
            {
                // Call the next middleware in the pipeline
                await _next(context);

                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                
                var status = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300 ? "Success" : "Failed";

               
                var additionalData = JsonSerializer.Serialize(new
                {
                    DurationMs = duration.TotalMilliseconds,
                    ResponseStatusCode = context.Response.StatusCode,
                    ResponseContentType = context.Response.ContentType,
                    ResponseContentLength = context.Response.ContentLength
                });

               
                var resourceType = ExtractResourceType(context.Request.Path.ToString());

            
                await auditService.LogAuditEventAsync(
                    action: context.Request.Method,
                    resourceType: resourceType,
                    resourceId: null,
                    description: $"API call to {context.Request.Path} completed in {duration.TotalMilliseconds:F2}ms",
                    status: status,
                    additionalData: additionalData
                );

                _logger.LogInformation("Audit logged for {Method} {Path} - Status: {StatusCode}, Duration: {Duration}ms",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                var endTime = DateTime.UtcNow;
                var duration = endTime - startTime;

                
                var additionalData = JsonSerializer.Serialize(new
                {
                    DurationMs = duration.TotalMilliseconds,
                    Exception = ex.Message,
                    StackTrace = ex.StackTrace
                });

               
                var resourceType = ExtractResourceType(context.Request.Path.ToString());

                await auditService.LogAuditEventAsync(
                    action: context.Request.Method,
                    resourceType: resourceType,
                    resourceId: null,
                    description: $"API call to {context.Request.Path} failed after {duration.TotalMilliseconds:F2}ms",
                    status: "Failed",
                    additionalData: additionalData
                );

                _logger.LogError(ex, "Audit logged for failed request {Method} {Path} - Duration: {Duration}ms",
                    context.Request.Method, context.Request.Path, duration.TotalMilliseconds);

               
                throw;
            }
        }
    }

    private static string ExtractResourceType(string path)
    {
    
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 1 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase)
            ? segments[1]
            : "API";
    }
}