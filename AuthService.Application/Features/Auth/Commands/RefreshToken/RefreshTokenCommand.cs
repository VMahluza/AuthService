using MediatR;

namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResult>;
