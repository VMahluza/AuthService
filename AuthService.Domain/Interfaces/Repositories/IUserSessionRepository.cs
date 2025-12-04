using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IUserSessionRepository : IRepository<UserSession>
{
    Task<UserSession?> GetActiveSessionByTokenAsync(string jwtToken);
    Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdPageAsync(Guid userId, int pageNumber, int pageSize);
    
    /// <summary>
    /// Gets all active sessions for a user (non-expired and not revoked)
    /// </summary>
    Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdAsync(Guid userId);
    
    /// <summary>
    /// Gets count of active sessions for a user
    /// </summary>
    Task<int> GetActiveSessionsCountAsync(Guid userId);
}

