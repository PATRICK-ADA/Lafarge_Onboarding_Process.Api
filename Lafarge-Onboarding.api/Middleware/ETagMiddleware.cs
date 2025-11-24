namespace Lafarge_Onboarding.api.Middleware;

public sealed class ETagMiddleware
{
    private readonly RequestDelegate _next;

    public ETagMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        if (context.Response.StatusCode == 200 && context.Request.Method == "GET")
        {
            var body = responseBody.ToArray();
            var etag = GenerateETag(body);
            
            context.Response.Headers.ETag = etag;
            
            if (context.Request.Headers.IfNoneMatch == etag)
            {
                context.Response.StatusCode = 304;
                context.Response.Body = originalBodyStream;
                return;
            }
        }

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
        context.Response.Body = originalBodyStream;
    }

    private static string GenerateETag(byte[] data)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(data);
        return $"\"{Convert.ToBase64String(hash)}\"";
    }
}