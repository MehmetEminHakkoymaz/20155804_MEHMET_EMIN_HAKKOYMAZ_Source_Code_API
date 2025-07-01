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
                    Message = _environment.IsDevelopment() ? error.Message : "Sunucu hatasý oluþtu.",
                    Details = _environment.IsDevelopment() ? error.StackTrace : null
                };

                switch (error)
                {
                    case KeyNotFoundException:
                        // Bulunamadý hatasý
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        _logger.LogWarning("Kaynak bulunamadý: {Message}", error.Message);
                        break;

                    case UnauthorizedAccessException:
                        // Yetkisiz eriþim hatasý
                        response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        _logger.LogWarning("Yetkisiz eriþim: {Message}", error.Message);
                        break;

                    case ArgumentException:
                    case FormatException:
                        // Geçersiz veri formatý veya argüman 
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        _logger.LogWarning("Geçersiz istek: {Message}", error.Message);
                        break;

                    case InvalidOperationException:
                        // Ýþ mantýðý hatalarý
                        response.StatusCode = (int)HttpStatusCode.Conflict;
                        _logger.LogWarning("Ýþ mantýðý hatasý: {Message}", error.Message);
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