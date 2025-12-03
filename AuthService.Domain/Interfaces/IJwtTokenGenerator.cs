using AuthService.Domain.DTOs;

namespace AuthService.Domain.Interfaces;

public interface IJwtTokenGenerator
{
    AuthenticationResult GenerateToken(Guid userId, string userName, string email);
}