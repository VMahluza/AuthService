using AuthService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces;

public interface IPasswordHasher
{
    PasswordHash Hash(string password);
    bool Verify(string password, PasswordHash passwordHash);
}
