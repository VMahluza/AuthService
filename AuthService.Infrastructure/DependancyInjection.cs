using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using AuthService.Infrastructure.Database;  // Add this for IAuthConnectionFactory and AuthConnectionFactory

namespace AuthService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the connection factory for better abstraction
        services.AddSingleton<IAuthConnectionFactory>(provider =>
            new AuthConnectionFactory(configuration.GetConnectionString("Default")));
        
        // Optionally, keep transient connection if needed directly, but prefer factory
        services.AddTransient(provider =>
            provider.GetRequiredService<IAuthConnectionFactory>().CreateConnection());
        
        return services;
    }
}
