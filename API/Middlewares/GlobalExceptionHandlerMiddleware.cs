using Microsoft.EntityFrameworkCore;

namespace AskMeAI.API.Middlewares;
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "A database update exception occurred.");

            var postgresException = dbEx.InnerException as Npgsql.PostgresException;
            if (postgresException != null && postgresException.SqlState == "23503")
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Foreign key constraint violation.",
                    message = "The related conversation does not exist. Please provide a valid conversation ID."
                });
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "A database update error occurred.",
                    message = dbEx.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred.",
                message = ex.Message
            });
        }
    }
}
