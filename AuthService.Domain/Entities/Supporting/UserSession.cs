using AuthService.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthService.Domain.Entities.Supporting;
public class UserSession : BaseEntity
{
    public Guid UserId { get; private set; }

    public string? JwtToken { get; private set; }

    public DateTime IssuedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// If null, the session is valid. If set, the session was manually killed (Logout).
    /// </summary>
    public DateTime? RevokedAt { get; private set; }
    public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

    private UserSession() { }

    public static UserSession Create(Guid userId, string refreshToken, DateTime expiresAt)
    {
          return new UserSession
        {
            UserId = userId,
            JwtToken = refreshToken,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            RevokedAt = null
          };
    }

    public void Revoke()
    {
        if (RevokedAt == null)
        {
            RevokedAt = DateTime.UtcNow;
        }
    }
}
