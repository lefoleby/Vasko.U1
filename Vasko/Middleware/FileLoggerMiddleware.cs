using Serilog;

namespace Vasko.Middleware
{
    public class FileLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public FileLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await _next(httpContext);

            var code = httpContext.Response.StatusCode;
            var temp = code / 100;

            if (temp != 2)
            {
                Log.Information($"--> Request {httpContext.Request.Path} returns {code}");
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class FileLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseFileLoggerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FileLoggerMiddleware>();
        }
    }
}