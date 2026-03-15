using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
namespace ResumeBuilder.Application.Common.Behaviors;
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) => _logger = logger;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var response = await next();
        sw.Stop();
        if (sw.ElapsedMilliseconds > 500)
            _logger.LogWarning("Slow request: {Name} ({Ms}ms)", typeof(TRequest).Name, sw.ElapsedMilliseconds);
        return response;
    }
}
