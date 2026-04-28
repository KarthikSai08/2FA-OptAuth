using Application.Dtos;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Net;
using System.Text.Json;

namespace Api.Middleware
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, " Unhandled Exception on {Method} {path}", context.Request.Method, context.Request.Path);
                await WriteErrorAsync(context, ex);
            }
        }

        private static Task WriteErrorAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var body = JsonSerializer.Serialize(new ApiResponse<object>(
                false,
                "An Unexpected error Occurred",
                null));

            return context.Response.WriteAsync(body);
        }
    }
}
