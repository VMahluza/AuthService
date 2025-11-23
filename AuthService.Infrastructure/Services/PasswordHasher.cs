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

        public bool Verify(string password, PasswordHash passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash.Value);
        }
    }
}