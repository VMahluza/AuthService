namespace AuthService.API.Contracts.Auth.Logout;

public record LogoutUserRequest(
    string JwtToken,
    bool RevokeAllSessions
    );