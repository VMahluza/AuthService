using AuthService.Domain.Entities.User;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IUserRoleRepository
{
    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    Task AddAsync(UserRole userRole);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    Task RemoveAsync(Guid userId, Guid roleId);

    /// <summary>
    /// Gets all roles for a user
    /// </summary>
    Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId);

    /// <summary>
    /// Gets all users with a specific role
    /// </summary>
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId);

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    Task<bool> HasRoleAsync(Guid userId, Guid roleId);

    /// <summary>
    /// Removes all role assignments for a user
    /// </summary>
    Task RemoveAllByUserIdAsync(Guid userId);

    /// <summary>
    /// Removes all user assignments for a role
    /// </summary>
    Task RemoveAllByRoleIdAsync(Guid roleId);
}
