using AuthService.Domain.DTOs;
using AuthService.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
   

    private UserSession() { }

    public UserSession(Guid id, Guid userId, AuthenticationResult authenticationResult, DateTime expiresAt)
        : base(id)  // ✅ Add this to call BaseEntity constructor
    {
        // Remove: Id = id;  // Already set by base(id)
        UserId = userId;
        JwtToken = authenticationResult.AccessToken;
        IssuedAt = DateTime.UtcNow;
        ExpiresAt = expiresAt;
        RevokedAt = null;
    }


    public static UserSession Create(Guid userId, AuthenticationResult authenticationResult, DateTime expiresAt)
    {
        return new UserSession(
            Guid.NewGuid(),
            userId,
            authenticationResult,
            expiresAt
        );
    }

    public void Revoke()
    {
        if (RevokedAt == null)
        {
            RevokedAt = DateTime.UtcNow;
        }
    }

    public bool IsActiveSession()
    {
        return RevokedAt == null && DateTime.UtcNow < ExpiresAt;
    }
}
