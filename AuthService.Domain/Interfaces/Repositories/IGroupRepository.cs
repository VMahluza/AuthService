using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IGroupRepository : IRepository<AuthGroup>
{
    /// <summary>
    /// Gets a group by its unique name
    /// </summary>
    Task<AuthGroup?> GetByNameAsync(string name);

    /// <summary>
    /// Checks if a group name already exists
    /// </summary>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// Gets all groups with pagination
    /// </summary>
    Task<IEnumerable<AuthGroup>> GetAllAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Gets count of users in this group
    /// </summary>
    Task<int> GetUserCountAsync(Guid groupId);
}
