using AuthService.Domain.Interfaces;
using System.Security.Cryptography;

namespace AuthService.Infrastructure.Services;

public class Base64TokenGenerator : IBase64TokenGenerator
{
    private const int TokenByteSize = 32; // 32 bytes = 256 bits

    public async Task<string> GenerateToken()
    {
        byte[] tokenBytes = RandomNumberGenerator.GetBytes(TokenByteSize);
        string token = Convert.ToBase64String(tokenBytes);
        
        return await Task.FromResult(token);
    }
}