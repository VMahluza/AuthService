using AuthService.Domain.Entities.User;

namespace AuthService.Domain.Interfaces.Repositories;

public interface IUserGroupRepository
{
    /// <summary>
    /// Adds a user to a group
    /// </summary>
    Task AddAsync(UserGroup userGroup);

    /// <summary>
    /// Removes a user from a group
    /// </summary>
    Task RemoveAsync(Guid userId, Guid groupId);

    /// <summary>
    /// Gets all groups for a user
    /// </summary>
    Task<IEnumerable<AuthGroup>> GetGroupsByUserIdAsync(Guid userId);

    /// <summary>
    /// Gets all users in a specific group
    /// </summary>
    Task<IEnumerable<User>> GetUsersByGroupIdAsync(Guid groupId);

    /// <summary>
    /// Checks if a user belongs to a specific group
    /// </summary>
    Task<bool> IsMemberAsync(Guid userId, Guid groupId);

    /// <summary>
    /// Removes all group memberships for a user
    /// </summary>
    Task RemoveAllByUserIdAsync(Guid userId);

    /// <summary>
    /// Removes all user memberships for a group
    /// </summary>
    Task RemoveAllByGroupIdAsync(Guid groupId);
}
