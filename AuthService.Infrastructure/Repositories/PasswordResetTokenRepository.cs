using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;

namespace AuthService.Infrastructure.Repositories;

public class PasswordResetTokenRepository : TokenRepository<PasswordResetToken>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(IAuthConnectionFactory connectionFactory) 
        : base(connectionFactory, TokenType.PasswordReset)
    {
    }
}
