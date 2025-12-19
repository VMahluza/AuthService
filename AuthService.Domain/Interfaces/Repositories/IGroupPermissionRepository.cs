using AuthService.Domain.Entities.Supporting;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IGroupPermissionRepository : IRepository<GroupPermission>
{
    Task<bool> ExistsAsync(Guid groupId, Guid permissionId);
    Task<IEnumerable<GroupPermission>> GetByGroupIdAsync(Guid groupId);
    Task<IEnumerable<GroupPermission>> GetByPermissionIdAsync(Guid permissionId);
    Task DeleteByGroupAndPermissionAsync(Guid groupId, Guid permissionId);
    Task<int> CountByPermissionIdAsync(Guid permissionId);
}
