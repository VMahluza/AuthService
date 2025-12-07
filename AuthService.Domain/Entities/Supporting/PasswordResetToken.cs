using AuthService.Domain.Entities.Common;
using AuthService.Domain.Enums;

namespace AuthService.Domain.Entities.Supporting;

/// <summary>
/// Token sent when user requests a password reset.
/// </summary>
public class PasswordResetToken : ExpiringToken
{
    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private PasswordResetToken() { }

    public PasswordResetToken(
        Guid id,
        Guid userId,
        string token,
        DateTime expiresAt,
        DateTime? usedAt)
        : base(id, userId, token, TokenType.PasswordReset,  expiresAt, usedAt)
    {
    }
}
