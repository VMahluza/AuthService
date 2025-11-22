using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Register;

public record RegisterUserCommand(
    string UserName,
    string Email,
    string Password
    ) : IRequest<RegisterUserResult>;