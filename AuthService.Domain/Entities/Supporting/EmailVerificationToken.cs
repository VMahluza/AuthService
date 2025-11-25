using AuthService.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace AuthService.Domain.Entities.Supporting;
public class EmailVerificationToken : BaseEntity
{
    /// <summary>
    /// This is used after a new user registers – they get an email with a verification
    /// token link; when they use it, we confirm their email and possibly activate their account
    /// </summary>
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }

    //// for Dapper reconstitution
    private EmailVerificationToken() { }


    public EmailVerificationToken(
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


    /// <summary>
    /// Generates a secure random token
    /// </summary>
    /// <returns>A secure random token as a string.</returns>
    private string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var randomBytes = new byte[32];
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('='); ;
    }

}
