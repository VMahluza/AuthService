using AuthService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities.User;
public class User
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public EmailAddress Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; }
    private User() { }
    public User(string firstName, string lastName, EmailAddress email, string passwordHash)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
    }
}