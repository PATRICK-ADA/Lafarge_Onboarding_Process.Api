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
        context.Items["RequestStartTime"] = DateTime.UtcNow;

        await _next(context);
    }

    private static string MapHttpMethodToAction(string method)
    {
        return method.ToUpper() switch
        {
            "GET" => "READ",
            "POST" => "CREATE",
            "PUT" => "UPDATE",
            "DELETE" => "DELETE",
            _ => method
        };
    }

    private static string ExtractResourceType(string path)
    {

        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 1 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase)
            ? segments[1]
            : "API";
    }
}