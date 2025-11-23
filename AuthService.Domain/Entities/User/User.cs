using AuthService.Domain.Entities.Common;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;
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
    public User(Guid id, string userName, EmailAddress email, PasswordHash passwordHash, UserStatus status, int failedLoginAttempts) : base(id)
    {
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
        var newUserId = Guid.NewGuid();
        return new User(newUserId, userName, emailAddress, passwordHash, UserStatus.PendingVerification, 0);
    }
}