namespace AuthService.API.Contracts;

public record LogoutUserRequest(
    string JwtToken,
    bool RevokeAllSessions
    );