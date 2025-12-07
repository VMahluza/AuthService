using AuthService.Domain.Entities.Common;

namespace AuthService.Domain.Entities.Supporting;

public class PasswordResetToken : BaseEntity
{
    /// <summary>
    /// This is used when a user requests a password reset – they get an email with a reset
    /// token link; when they use it, we allow them to set a new password
    /// </summary>
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    //// for Dapper reconstitution
    private PasswordResetToken() { }

    public PasswordResetToken(
        Guid id,
        Guid userId,
        string token,
        DateTime expiresAt,
        DateTime? usedAt)
        : base(id)
    {
        UserId = userId;
        Token = token;         // Set the existing token from DB
        ExpiresAt = expiresAt; // Set the existing expiry from DB
        UsedAt = usedAt;
    }

    /// <summary>
    /// Marks the token as consumed if it is valid and not expired. 
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Consume()
    {
        if (UsedAt != null)
        {
            throw new InvalidOperationException("Token has already been used.");
        }
        if (DateTime.UtcNow > ExpiresAt)
        {
            throw new InvalidOperationException("Token has expired.");
        }
        UsedAt = DateTime.UtcNow;
    }

}
