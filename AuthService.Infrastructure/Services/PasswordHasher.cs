using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;
namespace AuthService.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public PasswordHash Hash(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return PasswordHash.Create(hashedPassword);
        }
    }
}