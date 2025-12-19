namespace AuthService.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
