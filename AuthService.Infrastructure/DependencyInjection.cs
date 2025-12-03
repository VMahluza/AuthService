using AuthService.Domain.Interfaces;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.Interfaces.Services;
using AuthService.Domain.Settings;
using AuthService.Infrastructure.Database;  // Add this for IAuthConnectionFactory and AuthConnectionFactory
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure;
// nx todo research 

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
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();


        return services;
    }
}
