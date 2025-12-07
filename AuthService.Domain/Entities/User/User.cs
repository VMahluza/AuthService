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

    public static User RegisterNew(string userName, string email, string hashedPassword)
    {

        Guid newUserId = Guid.NewGuid();

        EmailAddress emailAddress = EmailAddress.Create(email);
        PasswordHash passwordHash = PasswordHash.Create(hashedPassword);
        return new User(newUserId, userName, emailAddress, passwordHash, UserStatus.PendingVerification, 0);
    }

    public void UpdateLoginAttempts(bool isSuccess)
    {
        if (isSuccess)
        {
            FailedLoginAttempts = 0;
        }
        else
        {
            FailedLoginAttempts += 1;
        }
    }

    public void LockAccount()
    {
        Status = UserStatus.Locked;
    }
}