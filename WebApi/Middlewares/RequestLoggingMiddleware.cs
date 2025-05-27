using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions; // Adicione este using
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebApi.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var fullUrl = context.Request.GetDisplayUrl(); // URL completa
            var ip = context.Connection.RemoteIpAddress?.ToString();

            _logger.LogInformation("Tentativa de acesso: {Method} {Url} | IP: {IP}", method, fullUrl, ip);

            await _next(context);

            var statusCode = context.Response.StatusCode;
            _logger.LogInformation("Resposta: {Method} {Url} | Status: {StatusCode} | IP: {IP}", method, fullUrl, statusCode, ip);
        }
    }
}