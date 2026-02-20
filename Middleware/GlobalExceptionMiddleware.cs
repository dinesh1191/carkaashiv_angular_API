using carkaashiv_angular_API.DTOs;
using System.Net;
using System.Text.Json;

namespace carkaashiv_angular_API.Middleware
{
    public class GlobalExceptionMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;
        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {

            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occured");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new ErrorResponse
                {
                    Success = false,
                    Message = "Something went wrong."
                };

                if (_env.IsDevelopment())
                {
                    response.Message = ex.Message;
                    response.StackTrace = ex.StackTrace;
                }
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);




            }
        }
    }
}