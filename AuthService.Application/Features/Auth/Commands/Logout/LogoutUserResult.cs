namespace AuthService.Application.Features.Auth.Commands.Logout;
public record LogoutUserResult(
    bool IsSuccessful,
    string Message
    );