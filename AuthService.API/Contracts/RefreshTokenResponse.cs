namespace AuthService.API.Contracts;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
