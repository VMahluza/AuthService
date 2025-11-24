namespace AuthService.Domain.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string userName, string email);
}