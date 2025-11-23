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

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the connection factory for better abstraction
        string connectionString = configuration.GetConnectionString("Default");
        services.AddSingleton<IAuthConnectionFactory>(provider =>
            new AuthConnectionFactory(connectionString));

        // Optionally, keep transient connection if needed directly, but prefer factory
        services.AddTransient(provider =>
            provider.GetRequiredService<IAuthConnectionFactory>().CreateConnection());


        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        
        return services;
    }
}
