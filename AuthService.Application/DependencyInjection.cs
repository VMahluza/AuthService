using AuthService.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AuthService.Domain.Options;

namespace AuthService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

        services.AddOptions<SecuritySettingsOptions>()
            .BindConfiguration(SecuritySettingsOptions.SectionName)
            .Validate(options =>
                options.MaxFailedAccessAttempts > 0 &&
                options.DefaultLockoutTimeSpanInMinutes > 0,
                "Security Settings failed validation"
            );

        return services;
    }
}