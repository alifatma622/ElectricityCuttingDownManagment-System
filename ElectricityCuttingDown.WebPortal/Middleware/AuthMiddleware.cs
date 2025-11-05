namespace ElectricityCuttingDown.WebPortal.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            // Allow login without authentication
            if (path.StartsWithSegments("/Account/Login") || path.StartsWithSegments("/Account/Logout"))
            {
                await _next(context);
                return;
            }

            // Check if user is authenticated
            var userId = context.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.Redirect("/Account/Login");
                return;
            }

            await _next(context);
        }
    }
}
