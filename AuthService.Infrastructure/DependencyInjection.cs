using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using AuthService.Infrastructure.Database;  // Add this for IAuthConnectionFactory and AuthConnectionFactory
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Repositories;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Services;
using AuthService.Infrastructure.Settings;

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
     

        // Register services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}
