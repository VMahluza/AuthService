using AuthService.Domain.DTOs;

namespace AuthService.Domain.Interfaces;

public interface IJwtTokenGenerator
{
    Task<AuthenticationResult> GenerateToken(Guid userId, string userName, string email, IEnumerable<string> roles);
}