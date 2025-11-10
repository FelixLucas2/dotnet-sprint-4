using Microsoft.AspNetCore.Http;

namespace Sprint03.Security
{
    /// <summary>
    /// Simple API Key middleware. Looks for header "X-Api-Key" and compares with configuration "ApiKey".
    /// Skips Swagger and Health endpoints.
    /// </summary>
    public class ApiKeyMiddleware
    {
        public const string HeaderName = "X-Api-Key";
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            // Public endpoints do not require API key
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/swagger") || path.StartsWith("/health"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(HeaderName, out var providedKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            var expectedKey = configuration["ApiKey"];
            if (string.IsNullOrWhiteSpace(expectedKey) || !string.Equals(expectedKey, providedKey))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            await _next(context);
        }
    }
}
