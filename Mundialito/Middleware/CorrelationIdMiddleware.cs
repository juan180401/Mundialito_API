using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Infrastructure.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Add("X-Correlation-Id", correlationId);

        var start = DateTime.UtcNow;

        _logger.LogInformation(
            "Request iniciado {Method} {Path} CorrelationId: {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        await _next(context);

        var duration = DateTime.UtcNow - start;

        _logger.LogInformation(
            "Request finalizado {Method} {Path} Status {StatusCode} Duración {DurationMs}ms CorrelationId: {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            duration.TotalMilliseconds,
            correlationId);
    }
}