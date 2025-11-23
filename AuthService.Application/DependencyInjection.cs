using Microsoft.Extensions.DependencyInjection;
using MediatR;
using AuthService.Application.Features.Auth.Commands.Register;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
          
        return services;
    }
}