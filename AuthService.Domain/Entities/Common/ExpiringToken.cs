using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities.Common;

/// <summary>
/// Base class for time-limited, single-use tokens (email verification, password reset, etc.)
/// </summary>
public abstract class ExpiringToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    protected ExpiringToken() { }

    protected ExpiringToken(
        Guid id,
        Guid userId,
        string token,
        DateTime expiresAt,
        DateTime? usedAt)
        : base(id)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        UsedAt = usedAt;
    }

    /// <summary>
    /// Marks the token as consumed if it is valid and not expired.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when token is already used or expired.</exception>
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

    /// <summary>
    /// Checks if the token is valid (not used and not expired)
    /// </summary>
    public bool IsValid() => UsedAt == null && DateTime.UtcNow <= ExpiresAt;
}
