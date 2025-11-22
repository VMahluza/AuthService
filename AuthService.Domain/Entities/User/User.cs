using AuthService.Domain.Entities.Common;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities.User;
public class User : BaseEntity
{
    public string UserName { get; private set; }
    public EmailAddress Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    public UserStatus Status { get; private set; }

    public int FailedLoginAttempts { get; private set; }

    private User() { }
    public User(string userName, EmailAddress email, PasswordHash passwordHash, UserStatus status, int failedLoginAttempts)
    {
        Id = Guid.NewGuid();
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
        Status = status;
        FailedLoginAttempts = failedLoginAttempts;
    }

    public static User RegisterNew(string userName, string email, string rawPassword, IPasswordHasher hasher)
    {
        var emailAddress = EmailAddress.Create(email);
        var passwordHash = PasswordHash.Create(hasher.Hash(rawPassword).ToString());

        return new User(userName, emailAddress, passwordHash, UserStatus.PendingVerification, 0)
        {
            CreatedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        };
    }
}