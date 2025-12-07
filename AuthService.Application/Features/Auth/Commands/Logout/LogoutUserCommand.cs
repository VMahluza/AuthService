using MediatR;

namespace AuthService.Application.Features.Auth.Commands.Logout;
public record LogoutUserCommand(
    string JwtToken,
    bool RevokeAllSessions
    ) : IRequest<LogoutUserResult>;
