using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Hospital.Middleware
{
    public class GlobalExceptionHandler
    {
            private readonly RequestDelegate _next;
            private readonly ILogger<GlobalExceptionHandler> _logger;

            public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
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
                    _logger.LogError(ex, $" Exception caught: {ex.Message}");
                    await HandleExceptionAsync(context, ex);
                }
            }

            private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = exception switch
                {
                    ArgumentNullException => (int)HttpStatusCode.BadRequest,
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    ValidationException => (int)HttpStatusCode.BadRequest,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var errorResponse = new ErrorResponse
                {
                    StatusCode = response.StatusCode,
                    Message = exception.Message,
                    Details = exception.InnerException?.Message,
                    TimeStamp = DateTime.UtcNow
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
            }
        }

        public class ErrorResponse
        {
            public int StatusCode { get; set; }
            public string Message { get; set; } = string.Empty;
            public string? Details { get; set; }
            public DateTime TimeStamp { get; set; }
        }
 }

