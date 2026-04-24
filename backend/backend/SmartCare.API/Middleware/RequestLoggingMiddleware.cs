namespace SmartCare.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        _logger.LogInformation("Request {RequestId} started: {Method} {Path} from {RemoteIpAddress}", 
            requestId, context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress);

        try
        {
            await _next(context);
        }
        finally
        {
            var duration = DateTime.UtcNow - startTime;
            var logLevel = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
            
            _logger.Log(logLevel, "Request {RequestId} completed: {StatusCode} in {Duration}ms", 
                requestId, context.Response.StatusCode, duration.TotalMilliseconds);
        }
    }
}
