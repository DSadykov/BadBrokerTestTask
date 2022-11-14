using System;
using System.Net;
using System.Threading.Tasks;

using BadBrokerTestTask.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BadBrokerTestTask.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex.Message, HttpStatusCode.InternalServerError, "Something bad happened");
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, string exceptionMessage, HttpStatusCode httpStatusCode, string message)
        {

            _logger.LogError(exceptionMessage);
            HttpResponse response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)httpStatusCode;
            ErrorDTO errorDTO = new()
            {
                Message = message,
                StatusCode = (int)httpStatusCode
            };
            await response.WriteAsJsonAsync(errorDTO);
        }
    }
}
