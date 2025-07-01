using System.Net;
using System.Text.Json;

namespace ECommerceAPI.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ErrorHandlerMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlerMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var errorResponse = new
                {
                    Message = _environment.IsDevelopment() ? error.Message : "Sunucu hatas� olu�tu.",
                    Details = _environment.IsDevelopment() ? error.StackTrace : null
                };

                switch (error)
                {
                    case KeyNotFoundException:
                        // Bulunamad� hatas�
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        _logger.LogWarning("Kaynak bulunamad�: {Message}", error.Message);
                        break;

                    case UnauthorizedAccessException:
                        // Yetkisiz eri�im hatas�
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        _logger.LogWarning("Yetkisiz eri�im: {Message}", error.Message);
                        break;

                    case ArgumentException:
                    case FormatException:
                        // Ge�ersiz veri format� veya arg�man 
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogWarning("Ge�ersiz istek: {Message}", error.Message);
                        break;

                    case InvalidOperationException:
                        // �� mant��� hatalar�
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        _logger.LogWarning("�� mant��� hatas�: {Message}", error.Message);
                        break;

                    default:
                        // Bilinmeyen hatalar
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        _logger.LogError(error, "Beklenmeyen hata");
                        break;
                }

                var result = JsonSerializer.Serialize(errorResponse);
                await response.WriteAsync(result);
            }
        }
    }
    
    // Extension metodu
    public static class ErrorHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlerMiddleware>();
        }
    }
}