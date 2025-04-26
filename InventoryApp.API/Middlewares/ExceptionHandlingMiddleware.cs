using System.Net;
using System.Text.Json;
using InventoryApp.Domain.Contracts;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerService logger)
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
            _logger.LogError(ex.ToString());
            Console.WriteLine($"[EXCEPTION] {ex.Message}");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var errorResponse = new
            {
                message = "An unexpected error occurred.",
                detail = ex.Message,
                statusCode = context.Response.StatusCode
            };

            var errorJson = JsonSerializer.Serialize(errorResponse);

            await context.Response.WriteAsync(errorJson);
        }
    }
}
