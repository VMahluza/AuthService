using AuthService.Domain.Entities.User;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Reflection;

namespace AuthService.Infrastructure.Repositories;

public class UserGroupRepository : IUserGroupRepository
{
    private readonly IAuthConnectionFactory _connectionFactory;
    private const string TableName = "UserGroups";

    public UserGroupRepository(IAuthConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(UserGroup userGroup)
    {
        var sql = $@"
            INSERT INTO {TableName} (UserId, GroupId, AssignedAt, AssignedBy)
            VALUES (@UserId, @GroupId, @AssignedAt, @AssignedBy)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            UserId = userGroup.UserId,
            GroupId = userGroup.GroupId,
            AssignedAt = userGroup.AssignedAt,
            AssignedBy = userGroup.AssignedBy
        });
    }

    public async Task RemoveAsync(Guid userId, Guid groupId)
    {
        var sql = $"DELETE FROM {TableName} WHERE UserId = @UserId AND GroupId = @GroupId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId, GroupId = groupId });
    }

    public async Task<IEnumerable<AuthGroup>> GetGroupsByUserIdAsync(Guid userId)
    {
        var sql = $@"
            SELECT g.* 
            FROM Groups g
            INNER JOIN {TableName} ug ON g.Id = ug.GroupId
            WHERE ug.UserId = @UserId
            ORDER BY g.Name";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { UserId = userId });

        return results.Select(MapToGroup);
    }

    public async Task<IEnumerable<User>> GetUsersByGroupIdAsync(Guid groupId)
    {
        var sql = $@"
            SELECT u.* 
            FROM Users u
            INNER JOIN {TableName} ug ON u.Id = ug.UserId
            WHERE ug.GroupId = @GroupId
            ORDER BY u.UserName";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { GroupId = groupId });

        return results.Select(MapToUser);
    }

    public async Task<bool> IsMemberAsync(Guid userId, Guid groupId)
    {
        var sql = $"SELECT COUNT(1) FROM {TableName} WHERE UserId = @UserId AND GroupId = @GroupId";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, GroupId = groupId });
        return count > 0;
    }

    public async Task RemoveAllByUserIdAsync(Guid userId)
    {
        var sql = $"DELETE FROM {TableName} WHERE UserId = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task RemoveAllByGroupIdAsync(Guid groupId)
    {
        var sql = $"DELETE FROM {TableName} WHERE GroupId = @GroupId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { GroupId = groupId });
    }

    private AuthGroup MapToGroup(dynamic result)
    {
        var group = new AuthGroup(
            (Guid)result.Id,
            result.Name,
            result.Description
        );

        var createdAtProperty = typeof(AuthGroup).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(group, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(AuthGroup).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        if (result.LastUpdatedAt != null)
        {
            lastUpdatedAtProperty?.SetValue(group, (DateTime?)result.LastUpdatedAt);
        }

        return group;
    }

    private User MapToUser(dynamic result)
    {
        var user = new User(
            (Guid)result.Id,
            result.UserName,
            EmailAddress.Create(result.Email),
            PasswordHash.Create(result.PasswordHash),
            Enum.Parse<UserStatus>(result.Status),
            result.FailedLoginAttempts
        );

        var createdAtProperty = typeof(User).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(user, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(User).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        if (result.LastUpdatedAt != null)
        {
            lastUpdatedAtProperty?.SetValue(user, (DateTime?)result.LastUpdatedAt);
        }

        return user;
    }
}
