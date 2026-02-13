using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorshipManager.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IHostEnvironment environment)
    {
        _next = next; _logger = logger; _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception ex) { await HandleExceptionAsync(context, ex); }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        _logger.LogError(exception, "Unhandled error. TraceId: {TraceId}, Path: {Path}", traceId, context.Request.Path);

        var (statusCode, errorResponse) = exception switch
        {
            KeyNotFoundException => (HttpStatusCode.NotFound, new ErrorResponse { Success = false, Error = "Resource not found", Message = exception.Message, TraceId = traceId }),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, new ErrorResponse { Success = false, Error = "Unauthorized", Message = "You don't have permission.", TraceId = traceId }),
            InvalidOperationException => (HttpStatusCode.BadRequest, new ErrorResponse { Success = false, Error = "Invalid operation", Message = exception.Message, TraceId = traceId }),
            ArgumentException or ArgumentNullException => (HttpStatusCode.BadRequest, new ErrorResponse { Success = false, Error = "Invalid data", Message = exception.Message, TraceId = traceId }),
            _ => (HttpStatusCode.InternalServerError, new ErrorResponse { Success = false, Error = "Internal server error", Message = _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.", TraceId = traceId, Details = _environment.IsDevelopment() ? exception.ToString() : null })
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}

public class ErrorResponse
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) => app.UseMiddleware<GlobalExceptionMiddleware>();
}
