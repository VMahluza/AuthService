namespace AuthService.API.Contracts.Auth.RefreshToken;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
