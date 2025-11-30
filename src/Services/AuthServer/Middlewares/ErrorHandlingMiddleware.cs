using System.Text.Json;
using AuthServer.Domain.Exceptions;

namespace AuhtServer.Api.Middlewares
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred.");

                var statusCode = ex switch
                {
                    InvalidCreateException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    UserNotFoundException => StatusCodes.Status404NotFound,
                    _ => StatusCodes.Status500InternalServerError
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";
                var response = new
                {
                    StatusCode = statusCode,
                    Message = ex.Message

                };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }


    }
}
