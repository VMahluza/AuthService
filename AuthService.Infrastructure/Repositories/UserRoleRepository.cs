using AuthService.Domain.Entities.User;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Reflection;

namespace AuthService.Infrastructure.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly IAuthConnectionFactory _connectionFactory;
    private const string TableName = "UserRoles";

    public UserRoleRepository(IAuthConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(UserRole userRole)
    {
        var sql = $@"
            INSERT INTO {TableName} (UserId, RoleId, AssignedAt, AssignedBy)
            VALUES (@UserId, @RoleId, @AssignedAt, @AssignedBy)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            UserId = userRole.UserId,
            RoleId = userRole.RoleId,
            AssignedAt = userRole.AssignedAt,
            AssignedBy = userRole.AssignedBy
        });
    }

    public async Task RemoveAsync(Guid userId, Guid roleId)
    {
        var sql = $"DELETE FROM {TableName} WHERE UserId = @UserId AND RoleId = @RoleId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId, RoleId = roleId });
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId)
    {
        var sql = $@"
            SELECT r.* 
            FROM Roles r
            INNER JOIN {TableName} ur ON r.Id = ur.RoleId
            WHERE ur.UserId = @UserId
            ORDER BY r.Name";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { UserId = userId });

        return results.Select(MapToRole);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId)
    {
        var sql = $@"
            SELECT u.* 
            FROM Users u
            INNER JOIN {TableName} ur ON u.Id = ur.UserId
            WHERE ur.RoleId = @RoleId
            ORDER BY u.UserName";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { RoleId = roleId });

        return results.Select(MapToUser);
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid roleId)
    {
        var sql = $"SELECT COUNT(1) FROM {TableName} WHERE UserId = @UserId AND RoleId = @RoleId";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { UserId = userId, RoleId = roleId });
        return count > 0;
    }

    public async Task RemoveAllByUserIdAsync(Guid userId)
    {
        var sql = $"DELETE FROM {TableName} WHERE UserId = @UserId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { UserId = userId });
    }

    public async Task RemoveAllByRoleIdAsync(Guid roleId)
    {
        var sql = $"DELETE FROM {TableName} WHERE RoleId = @RoleId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { RoleId = roleId });
    }

    private Role MapToRole(dynamic result)
    {
        var role = new Role(
            (Guid)result.Id,
            result.Name,
            result.Description
        );

        var createdAtProperty = typeof(Role).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(role, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(Role).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        if (result.LastUpdatedAt != null)
        {
            lastUpdatedAtProperty?.SetValue(role, (DateTime?)result.LastUpdatedAt);
        }

        return role;
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
