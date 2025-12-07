using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using AuthService.Domain.Settings;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Enums;
using AuthService.Infrastructure.Database;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Default");
        services.AddSingleton<IAuthConnectionFactory>(provider =>
            new AuthConnectionFactory(connectionString));

        services.AddTransient(provider =>
            provider.GetRequiredService<IAuthConnectionFactory>().CreateConnection());

        services.Configure<JwtSettingsOptions>(configuration.GetSection(JwtSettingsOptions.SectionName));

        services.AddOptions<JwtSettingsOptions>()
            .BindConfiguration(JwtSettingsOptions.SectionName)
            .Validate(options =>
                !string.IsNullOrEmpty(options.Secret) &&
                !string.IsNullOrEmpty(options.Issuer) &&
                !string.IsNullOrEmpty(options.Audience) &&
                options.ExpiryMinutes > 0,
                "JWT Settings failed validation"
            );

        // Register services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IAuthEmailSender, AuthEmailSender>();
        services.AddScoped<IServerAddress, ServerAddress>();
        services.AddScoped<IBase64TokenGenerator, Base64TokenGenerator>(); 

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Register token repositories with specialized implementations
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<ITokenRepository<EmailVerificationToken>>(provider =>
            provider.GetRequiredService<IEmailVerificationTokenRepository>());
        
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<ITokenRepository<PasswordResetToken>>(provider =>
            provider.GetRequiredService<IPasswordResetTokenRepository>());

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();

        // Register role and group repositories
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();

        return services;
    }
}
