namespace AuthService.API.Contracts.Auth.Logout;

public record LogoutUserResponse(
    bool IsSuccessful,
    string Message
    );
