using SmartCare.Application.Common.DTOs;
using System.Text.Json;

namespace SmartCare.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ArgumentException => CreateErrorResponse(context.Response.StatusCode, exception.Message),
            InvalidOperationException => CreateErrorResponse(StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedAccessException => CreateErrorResponse(StatusCodes.Status401Unauthorized, "Unauthorized access"),
            KeyNotFoundException => CreateErrorResponse(StatusCodes.Status404NotFound, "Resource not found"),
            _ => CreateErrorResponse(StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response.ApiResponse));
    }

    private static (int StatusCode, object ApiResponse) CreateErrorResponse(int statusCode, string message)
    {
        var apiResponse = statusCode switch
        {
            StatusCodes.Status400BadRequest => ApiResponse.ErrorResult(message),
            StatusCodes.Status401Unauthorized => ApiResponse.ErrorResult(message),
            StatusCodes.Status404NotFound => ApiResponse.ErrorResult(message),
            _ => ApiResponse.ErrorResult("An unexpected error occurred")
        };

        return (statusCode, apiResponse);
    }
}
