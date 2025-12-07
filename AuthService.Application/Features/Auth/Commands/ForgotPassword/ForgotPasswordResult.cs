namespace AuthService.Application.Features.Auth.Commands.ForgotPassword;

public record ForgotPasswordResult(
    bool Success,
    string Message
);
