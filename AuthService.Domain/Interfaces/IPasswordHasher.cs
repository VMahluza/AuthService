using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
}
