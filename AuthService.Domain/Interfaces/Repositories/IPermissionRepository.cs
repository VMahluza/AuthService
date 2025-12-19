using AuthService.Domain.Entities.User;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission?> GetByKeyAsync(string key);
    Task<bool> ExistsByKeyAsync(string key);
    Task<IEnumerable<Permission>> GetAllAsync();
    Task<IEnumerable<Permission>> GetByGroupIdAsync(Guid groupId);
    Task<IEnumerable<Permission>> GetEffectivePermissionsByUserIdAsync(Guid userId);
}
