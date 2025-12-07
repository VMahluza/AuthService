using AuthService.Domain.Entities.Common;
using AuthService.Domain.Entities.Supporting;
using System.Data;

namespace AuthService.Domain.Interfaces.Repositories;

public interface ITokenRepository<T> : IRepository<T> where T : ExpiringToken
{
    Task<T?> GetByTokenAsync(string token);
    Task<IEnumerable<T>> GetByUserIdAsync(Guid userId);
    Task InvalidateAllForUserAsync(Guid userId);
}
