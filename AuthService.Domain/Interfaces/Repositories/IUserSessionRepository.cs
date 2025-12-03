using AuthService.Domain.Entities.Supporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IUserSessionRepository : IRepository<UserSession>
{
    Task<UserSession?> GetActiveSessionByTokenAsync(string jwtToken);
    Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdPageAsync(Guid userId, int pageNumber, int pageSize);
}

