using carkaashiv_angular_API.DTOs;
using System;
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
                context.Response.StatusCode = ex switch
                {
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    _ => (int)HttpStatusCode.InternalServerError
                };
                var response = new ErrorResponse
                {
                    Success = false,
                    Message = _env.IsDevelopment()?
                    ex.InnerException?.Message ?? ex.Message: "Something went wrong.",
                    StackTrace = _env.IsDevelopment() ? ex.StackTrace : null
                };
                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
               
            }
        }
    }
}