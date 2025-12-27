using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthService.Domain.Constants;

namespace AuthService.Infrastructure.Authentication;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];

        if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
        {
            throw new InvalidOperationException(
                "JWT Secret key must be at least 32 characters long. " +
                "Please set a secure key via environment variable or user secrets.");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.RequireAdminRole, policy =>
                policy.RequireRole("Admin"));
            
            options.AddPolicy(AuthorizationPolicies.RequireSuperAdminRole, policy =>
                policy.RequireRole("SuperAdmin"));
            
            options.AddPolicy(AuthorizationPolicies.RequireManagerRole, policy =>
                policy.RequireRole("Manager"));
            
            options.AddPolicy(AuthorizationPolicies.RequireUserRole, policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy(AuthorizationPolicies.CanManageRoles, policy =>
                policy.RequireRole("Admin", "SuperAdmin"));
            
            options.AddPolicy(AuthorizationPolicies.CanManageGroups, policy =>
                policy.RequireRole("Admin", "SuperAdmin"));
            
            options.AddPolicy(AuthorizationPolicies.CanManageUsers, policy =>
                policy.RequireRole("Admin", "Manager", "SuperAdmin"));
            
            options.AddPolicy(AuthorizationPolicies.CanViewAuditLogs, policy =>
                policy.RequireRole("Admin", "Manager", "SuperAdmin"));
        });

        return services;
    }
}
