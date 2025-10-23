using ApiMonetizationGateway.Services;

namespace ApiMonetizationGateway.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Resolve scoped service from the request's IServiceProvider
            var rateLimit = context.RequestServices.GetRequiredService<IRateLimitService>();

            var customerId = context.Request.Headers["X-Customer-Id"].FirstOrDefault();
            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var endpoint = context.Request.Path.ToString();

            var result = await rateLimit.ValidateRequestAsync(customerId ?? string.Empty, endpoint, userId);
            if (!result.Allowed)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync(new { error = result.Message });
                return;
            }

            await _next(context);
        }
    }
}
