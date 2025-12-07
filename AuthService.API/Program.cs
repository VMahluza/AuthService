using MySqlConnector;
using AuthService.Infrastructure;
using AuthService.Application;  // Add this for AddApplication
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Role-based policies
    options.AddPolicy(AuthorizationPolicies.RequireAdminRole, policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy(AuthorizationPolicies.RequireManagerRole, policy =>
        policy.RequireRole("Admin", "Manager"));
    
    options.AddPolicy(AuthorizationPolicies.RequireUserRole, policy =>
        policy.RequireAuthenticatedUser());

    // Permission-based policies (for future use with claims)
    options.AddPolicy(AuthorizationPolicies.CanManageRoles, policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy(AuthorizationPolicies.CanManageGroups, policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy(AuthorizationPolicies.CanManageUsers, policy =>
        policy.RequireRole("Admin", "Manager"));
    
    options.AddPolicy(AuthorizationPolicies.CanViewAuditLogs, policy =>
        policy.RequireRole("Admin", "Manager"));
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

app.UseHttpsRedirection();

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
