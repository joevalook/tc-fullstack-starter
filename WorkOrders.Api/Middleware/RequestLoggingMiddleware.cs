using System.Diagnostics;

namespace WorkOrders.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // BEFORE the next middleware (incoming request)
        _logger.LogInformation(
            "Incoming Request: {method} {path}",
            context.Request.Method,
            context.Request.Path
        );

        await _next(context);

        stopwatch.Stop();

        // AFTER the next middleware (response)
        _logger.LogInformation(
            "Outgoing Response: {statusCode} in {elapsed}ms",
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds
        );
    }
}