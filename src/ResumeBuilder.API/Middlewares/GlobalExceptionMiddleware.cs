using ResumeBuilder.Domain.Exceptions;
using System.Net;
using System.Text.Json;
namespace ResumeBuilder.API.Middlewares;
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment env) { _next = next; _logger = logger; _env = env; }
    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception ex) { _logger.LogError(ex, "Unhandled exception"); await HandleAsync(context, ex); }
    }
    private async Task HandleAsync(HttpContext ctx, Exception ex)
    {
        ctx.Response.ContentType = "application/json";
        var r = new ErrorResponse();
        switch (ex)
        {
            case ValidationException vx: ctx.Response.StatusCode = 400; r.StatusCode = 400; r.Message = "Validation failed."; r.Errors = vx.Errors; break;
            case NotFoundException nx: ctx.Response.StatusCode = 404; r.StatusCode = 404; r.Message = nx.Message; break;
            case UnauthorizedException ux: ctx.Response.StatusCode = 401; r.StatusCode = 401; r.Message = ux.Message; break;
            case ForbiddenAccessException fx: ctx.Response.StatusCode = 403; r.StatusCode = 403; r.Message = fx.Message; break;
            case BadRequestException bx: ctx.Response.StatusCode = 400; r.StatusCode = 400; r.Message = bx.Message; break;
            case ConflictException cx: ctx.Response.StatusCode = 409; r.StatusCode = 409; r.Message = cx.Message; break;
            default: ctx.Response.StatusCode = 500; r.StatusCode = 500; r.Message = _env.IsDevelopment() ? ex.Message : "Internal server error."; r.Detail = _env.IsDevelopment() ? ex.StackTrace : null; break;
        }
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(r, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
public class ErrorResponse { public int StatusCode { get; set; } public string Message { get; set; } = ""; public string? Detail { get; set; } public IDictionary<string, string[]>? Errors { get; set; } public DateTime Timestamp { get; set; } = DateTime.UtcNow; }
