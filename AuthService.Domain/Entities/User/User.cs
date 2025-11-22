using AuthService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities.User;
public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string UserName { get; private set; }
    public EmailAddress Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }

    public string FullName => $"{FirstName} {LastName}";
    private User() { }
    public User(string firstName, string lastName, string userName, EmailAddress email, PasswordHash passwordHash)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        UserName = userName;
        Email = email;
        PasswordHash = passwordHash;
    }

    public static User RegisterNew(string firstName, string lastName, string userName, string email, string rawPassword, IPasswordHasher hasher)
    {
        var emailAddress = EmailAddress.Create(email);
        var passwordHash = PasswordHash.Create(hasher.Hash(rawPassword).ToString());
        return new User(firstName, lastName, userName, emailAddress, passwordHash);
    }
}