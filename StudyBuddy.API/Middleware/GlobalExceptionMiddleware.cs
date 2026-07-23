using System.Net;
using System.Text.Json;
using StudyBuddy.BLL.Exceptions;

namespace StudyBuddy.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DuplicateEmailException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.Conflict,
                    ex.Message);
            }
            catch (InvalidCredentialsException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.Unauthorized,
                    ex.Message);
            }
            catch (UserNotFoundException ex)
            {
                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            //catch (ExternalAuthenticationException ex)
            //{
            //    await HandleExceptionAsync(
            //        context,
            //        HttpStatusCode.Unauthorized,
            //        ex.Message);
            //}
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                await HandleExceptionAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "Something went wrong. Please try again later.");
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                statusCode = (int)statusCode,
                message = message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}