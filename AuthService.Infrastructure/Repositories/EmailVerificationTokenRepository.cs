using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;

namespace AuthService.Infrastructure.Repositories;

public class EmailVerificationTokenRepository : TokenRepository<EmailVerificationToken>, IEmailVerificationTokenRepository
{
    public EmailVerificationTokenRepository(IAuthConnectionFactory connectionFactory) 
        : base(connectionFactory, TokenType.EmailVerification)
    {
    }
}
