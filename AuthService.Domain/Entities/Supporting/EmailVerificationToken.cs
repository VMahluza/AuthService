using AuthService.Domain.Entities.Common;
using AuthService.Domain.Enums;
namespace AuthService.Domain.Entities.Supporting;
/// <summary>
/// Token sent after user registration to verify email address.
/// </summary>
public class EmailVerificationToken : ExpiringToken
{
    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private EmailVerificationToken() { }

    public EmailVerificationToken(
        Guid id,
        Guid userId,
        string token,
        DateTime expiresAt,
        DateTime? usedAt)
        : base(id, userId, token, TokenType.EmailVerification, expiresAt, usedAt)
    {
    }
}
