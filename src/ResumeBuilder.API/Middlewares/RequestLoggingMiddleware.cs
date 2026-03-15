namespace ResumeBuilder.API.Middlewares;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger) { _next = next; _logger = logger; }
    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;
        await _next(context);
        _logger.LogInformation("HTTP {Method} {Path} => {StatusCode} in {Ms:0}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, (DateTime.UtcNow - start).TotalMilliseconds);
    }
}
