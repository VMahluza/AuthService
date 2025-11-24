using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AuthService.Infrastructure.Repositories;
public class EmailVerificationTokenRepository : BaseRepository<EmailVerificationToken>, IEmailVerificationTokenRepository
{
    public EmailVerificationTokenRepository(IAuthConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public Task AddAsync(EmailVerificationToken token, IDbTransaction? transaction = null)
    {
        throw new NotImplementedException();
    }

    public Task<EmailVerificationToken?> GetByTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    protected override EmailVerificationToken MapToEntity(dynamic result)
    {
        throw new NotImplementedException();
    }
}