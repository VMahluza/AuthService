using AuthService.Domain.Entities.Supporting;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IPasswordResetTokenRepository : ITokenRepository<PasswordResetToken>
{
    // All methods are inherited from ITokenRepository<PasswordResetToken>
    // No need to redeclare AddAsync and GetByTokenAsync
}
