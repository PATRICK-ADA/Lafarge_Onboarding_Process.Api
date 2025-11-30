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

        if (context.Response.StatusCode == 200 && (context.Request.Method == "GET" || context.Request.Method == "HEAD"))
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            var etag = GenerateETagIncremental(responseBody);
            
            context.Response.Headers.ETag = etag;
            
            if (IsETagMatch(context.Request.Headers.IfNoneMatch.ToString(), etag))
            {
                context.Response.StatusCode = 304;
                context.Response.Headers.Remove("Content-Length");
                context.Response.Body = originalBodyStream;
                return;
            }
            
            responseBody.Seek(0, SeekOrigin.Begin);
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

    private static string GenerateETagIncremental(Stream stream)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(stream);
        return $"\"{Convert.ToBase64String(hash)}\"";
    }

    private static bool IsETagMatch(string ifNoneMatchHeader, string etag)
    {
        if (ifNoneMatchHeader == null) return false;
        if (string.IsNullOrEmpty(ifNoneMatchHeader)) return false;
        if (ifNoneMatchHeader.Trim() == "*") return true;

        var etags = ifNoneMatchHeader.Split(',').Select(e => e.Trim()).ToArray();
        foreach (var e in etags)
        {
            var normalizedE = e;
            if (normalizedE.StartsWith("W/", StringComparison.OrdinalIgnoreCase))
            {
                normalizedE = normalizedE.Substring(2);
            }
            if (normalizedE == etag)
            {
                return true;
            }
        }
        return false;
    }
}