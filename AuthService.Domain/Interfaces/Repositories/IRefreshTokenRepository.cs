using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : ITokenRepository<RefreshToken>
{
}
