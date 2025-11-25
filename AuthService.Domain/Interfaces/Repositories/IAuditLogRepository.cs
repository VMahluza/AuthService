using AuthService.Domain.Entities.Supporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task AddAsync(AuditLog auditLog);
    Task<IEnumerable<AuditLog>> GetPagedAsync(int pageNumber, int pageSize);

}
