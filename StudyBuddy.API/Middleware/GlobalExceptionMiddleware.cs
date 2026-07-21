using System.Net;
using System.Text.Json;
using StudyBuddy.BLL.Exceptions;

namespace StudyBuddy.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine("===== MIDDLEWARE STARTED =====");

        try
        {
            await _next(context);
        }
        catch (DuplicateEmailException ex)
        {
            Console.WriteLine("===== DUPLICATE EMAIL CAUGHT =====");

            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            }));
        }
        catch (InvalidCredentialsException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            }));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                success = false,
                message = ex.Message
            }));
        }
    }
}