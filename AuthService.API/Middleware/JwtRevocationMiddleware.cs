using AuthService.Domain.Interfaces.Repositories;

namespace AuthService.API.Middleware;

/// <summary>
/// Middleware that validates JWT tokens against the active session database
/// to ensure revoked tokens cannot be used even if not expired
/// </summary>
public class JwtRevocationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtRevocationMiddleware> _logger;

    public JwtRevocationMiddleware(RequestDelegate next, ILogger<JwtRevocationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IUserSessionRepository sessionRepository)
    {
        // Skip if not authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            await _next(context);
            return;
        }

        // Extract JWT from Authorization header
        var authHeader = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token))
        {
            await _next(context);
            return;
        }

        // Check if session is still active in database
        var session = await sessionRepository.GetActiveSessionByTokenAsync(token);
        
        if (session == null)
        {
            _logger.LogWarning("Token validation failed: Session not found or revoked for token");
            
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            
            await context.Response.WriteAsJsonAsync(new 
            { 
                error = "Unauthorized",
                message = "Token has been revoked or session expired. Please login again."
            });
            return;
        }

        await _next(context);
    }
}
