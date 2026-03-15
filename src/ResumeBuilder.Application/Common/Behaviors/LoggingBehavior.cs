using MediatR;
using Microsoft.Extensions.Logging;
namespace ResumeBuilder.Application.Common.Behaviors;
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _logger.LogInformation("Handling {Name}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {Name}", typeof(TRequest).Name);
        return response;
    }
}
