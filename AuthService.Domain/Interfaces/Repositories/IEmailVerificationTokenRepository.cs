using AuthService.Domain.Entities.Supporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IEmailVerificationTokenRepository : IRepository<EmailVerificationToken>
{
    Task AddAsync(EmailVerificationToken token, IDbTransaction? transaction = null);
    Task<EmailVerificationToken?> GetByTokenAsync(string token);
}