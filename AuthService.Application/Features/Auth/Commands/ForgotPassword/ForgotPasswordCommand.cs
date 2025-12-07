using MediatR;

namespace AuthService.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand(
    string Email
) : IRequest<ForgotPasswordResult>;
