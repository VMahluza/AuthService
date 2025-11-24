using AuthService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateToken(Guid userId, string userName, string email)
    {
        throw new NotImplementedException();
    }
}

