using AuthService.Domain.Entities.Supporting;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IEmailVerificationTokenRepository : ITokenRepository<EmailVerificationToken>
{
    // All methods are inherited from ITokenRepository<EmailVerificationToken>
    // No need to redeclare AddAsync and GetByTokenAsync
}