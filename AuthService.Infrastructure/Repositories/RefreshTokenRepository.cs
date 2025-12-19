using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;

namespace AuthService.Infrastructure.Repositories;

public class RefreshTokenRepository : TokenRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(IAuthConnectionFactory connectionFactory) 
        : base(connectionFactory, TokenType.RefreshToken)
    {
    }
}
