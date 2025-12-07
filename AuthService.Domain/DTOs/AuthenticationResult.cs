using System;

namespace AuthService.Domain.DTOs;

public record AuthenticationResult(
    string AccessToken,
    string RefreshToken,
    string Jti,
    DateTime ExpiresAt
);