using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// Gets a role by its unique name
    /// </summary>
    Task<Role?> GetByNameAsync(string name);

    /// <summary>
    /// Checks if a role name already exists
    /// </summary>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// Gets all roles with pagination
    /// </summary>
    Task<IEnumerable<Role>> GetAllAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Gets count of users assigned to this role
    /// </summary>
    Task<int> GetUserCountAsync(Guid roleId);
}
