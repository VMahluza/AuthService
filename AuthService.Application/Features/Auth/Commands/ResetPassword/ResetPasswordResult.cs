namespace AuthService.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordResult(
    bool Success,
    string Message
);
