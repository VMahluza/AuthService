namespace AuthService.Domain.Interfaces;
public interface IBase64TokenGenerator
{
    Task<string> GenerateToken();
}
