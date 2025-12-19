using AuthService.Domain.Entities.Common;
using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities.Supporting;

/// <summary>
/// Token used to obtain new access tokens without requiring re-authentication
/// </summary>
public class RefreshToken : ExpiringToken
{
    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private RefreshToken() { }

    public RefreshToken(
        Guid id,
        Guid userId,
        string token,
        DateTime expiresAt,
        DateTime? usedAt)
        : base(id, userId, token, TokenType.RefreshToken, expiresAt, usedAt)
    {
    }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        return new RefreshToken(
            Guid.NewGuid(),
            userId,
            token,
            expiresAt,
            null);
    }
}
