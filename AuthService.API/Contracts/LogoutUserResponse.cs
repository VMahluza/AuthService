namespace AuthService.API.Contracts;

public record LogoutUserResponse(
    bool IsSuccessful,
    string Message
    );
