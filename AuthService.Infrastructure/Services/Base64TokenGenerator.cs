using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Services;

public class Base64TokenGenerator : IBase64TokenGenerator
{
    public Task<string> GenerateToken()
    {
        throw new NotImplementedException();
    }
}