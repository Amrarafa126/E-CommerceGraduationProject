using E_Commerce.Core.Exceptions;
using System.Net;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace E_Commerce.Api.Middleware
{

    public class GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            var (statusCode, response) = exception switch
            {
                NotFoundException ex => (
                    HttpStatusCode.NotFound,
                    ApiResponse<object>.NotFound(ex.Message)),

                   ValidationException ex => (
                    HttpStatusCode.UnprocessableEntity,
                    ApiResponse<object>.ValidationFail(
                        ex.Errors.SelectMany(e => e.Value).ToList(),
                        "Validation failed.")),

                UnauthorizedException ex => (
                    HttpStatusCode.Unauthorized,
                    ApiResponse<object>.Unauthorized(ex.Message)),

                ForbiddenException ex => (
                    HttpStatusCode.Forbidden,
                    ApiResponse<object>.Forbidden(ex.Message)),

                ConflictException ex => (
                    HttpStatusCode.Conflict,
                    ApiResponse<object>.Fail(ex.Message, (int)HttpStatusCode.Conflict)),

                BusinessException ex => (
                    HttpStatusCode.BadRequest,
                    ApiResponse<object>.Fail(ex.Message, (int)HttpStatusCode.BadRequest)),

                _ => (
                    HttpStatusCode.InternalServerError,
                    ApiResponse<object>.Fail(
                        "An unexpected error occurred. Please try again later.",
                        (int)HttpStatusCode.InternalServerError))
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }

}
