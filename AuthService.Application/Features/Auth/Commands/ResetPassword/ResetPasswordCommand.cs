using MediatR;

namespace AuthService.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Token,
    string NewPassword
) : IRequest<ResetPasswordResult>;
