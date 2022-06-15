using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Middlewares
{
    public class ExceptionMiddleware
    {
        private RequestDelegate Next { get; }
        private ILogger<ExceptionMiddleware> Logger { get; }
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            Next = next;
            Logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, Logger);
            }
        }

        private static string SerializeObject<TValue>(TValue value)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            return JsonSerializer.Serialize(value, options);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ExceptionMiddleware> _logger)
        {
            context.Response.ContentType = "application/json";

            if (exception is ValidateException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync(SerializeObject(new IdentityError { Code = "ValidateError", Description = exception.Message }));
            }
            else if (exception is ConflictedException conflictedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                return context.Response.WriteAsync(SerializeObject(conflictedException.Conflicts()));
            }
            else if (exception is UnauthorizedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return context.Response.WriteAsync(SerializeObject(new IdentityError { Code = "UnauthorizeError", Description = exception.Message }));
            }
            else if (exception is ForbiddenedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return context.Response.WriteAsync(String.Empty);
            }
            else
            {
                _logger.LogError(exception.Message);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return context.Response.WriteAsync(SerializeObject(new IdentityError { Code = "InternalError", Description = exception.Message }));
            }
        }
    }
}
