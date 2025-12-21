using MySqlConnector;
using AuthService.Infrastructure;
using AuthService.Application;  // Add this for AddApplication
using AuthService.API.Middleware;
using AuthService.Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();  // Added for Swagger UI

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

// Validate JWT secret key minimum length (32 characters for HS256)
if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException(
        "JWT Secret key must be at least 32 characters long. " +
        "Please set a secure key via environment variable or user secrets.");
}

builder.Services.AddAuthentication(options =>
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
        ClockSkew = TimeSpan.FromMinutes(5), // Reduce clock skew tolerance
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Single role policies
    options.AddPolicy(AuthorizationPolicies.RequireAdminRole, policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy(AuthorizationPolicies.RequireSuperAdminRole, policy =>
        policy.RequireRole("SuperAdmin"));
    
    options.AddPolicy(AuthorizationPolicies.RequireManagerRole, policy =>
        policy.RequireRole("Manager"));
    
    options.AddPolicy(AuthorizationPolicies.RequireUserRole, policy =>
        policy.RequireAuthenticatedUser());

    // Permission-based policies (for future use with claims)
    options.AddPolicy(AuthorizationPolicies.CanManageRoles, policy =>
        policy.RequireRole("Admin", "SuperAdmin"));
    
    options.AddPolicy(AuthorizationPolicies.CanManageGroups, policy =>
        policy.RequireRole("Admin", "SuperAdmin"));
    
    options.AddPolicy(AuthorizationPolicies.CanManageUsers, policy =>
        policy.RequireRole("Admin", "Manager", "SuperAdmin"));
    
    options.AddPolicy(AuthorizationPolicies.CanViewAuditLogs, policy =>
        policy.RequireRole("Admin", "Manager", "SuperAdmin"));
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // React dev servers
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Layer registrations
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();  // Added for Swagger JSON
    app.UseSwaggerUI();  // Added for Swagger UI
}

// IMPORTANT: UseCors MUST come before UseHttpsRedirection
app.UseCors("AllowReactApp");

// Conditionally apply HTTPS redirection (not for preflight requests)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Add Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Append("Strict-Transport-Security", 
            "max-age=31536000; includeSubDomains");
    }
    
    await next();
});

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseMiddleware<JwtRevocationMiddleware>(); // Validate tokens against session database
app.UseAuthorization();

app.MapControllers();

app.Run();
