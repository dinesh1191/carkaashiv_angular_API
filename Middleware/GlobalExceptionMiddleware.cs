using carkaashiv_angular_API.DTOs;
using carkaashiv_angular_API.Exceptions;
using carkaashiv_angular_API.Models.Shared;

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
            catch(BusinessException ex)
            {      
                // Business validation errors
                _logger.LogWarning(ex, "Business Validation failed");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = ApiResponse<object>.Fail(ex.Message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                //System errors
                _logger.LogError(ex, "Unhandled exception occured");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    _ => (int)HttpStatusCode.InternalServerError
                };
                // include stacktrace only in dev
                var response = new ApiResponse<object>(
                    false,
                    _env.IsDevelopment() ? ex.InnerException?.Message ?? ex.Message
                    : "Something went wrong."
                    );

                var json = JsonSerializer.Serialize(new
                {
                    response.Success,
                    response.Message,
                    StarkTrace = _env.IsDevelopment()? ex.StackTrace :null
                });
                await context.Response.WriteAsync(json);
               
            }
        }
    }
}