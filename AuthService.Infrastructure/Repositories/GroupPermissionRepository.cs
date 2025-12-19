using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Reflection;

namespace AuthService.Infrastructure.Repositories;

public class GroupPermissionRepository : BaseRepository<GroupPermission>, IGroupPermissionRepository
{
    public GroupPermissionRepository(IAuthConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _tableName = "GroupPermissions";
    }

    public override async Task AddAsync(GroupPermission entity)
    {
        var sql = $@"
            INSERT INTO {_tableName} (Id, GroupId, PermissionId, AssignedByUserId, CreatedAt, LastUpdatedAt)
            VALUES (@Id, @GroupId, @PermissionId, @AssignedByUserId, @CreatedAt, @LastUpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            GroupId = entity.GroupId,
            PermissionId = entity.PermissionId,
            AssignedByUserId = entity.AssignedByUserId,
            CreatedAt = entity.CreatedAt,
            LastUpdatedAt = entity.LastUpdatedAt
        });
    }

    public override async Task UpdateAsync(GroupPermission entity)
    {
        var sql = $@"
            UPDATE {_tableName}
            SET LastUpdatedAt = @LastUpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            LastUpdatedAt = DateTime.UtcNow
        });
    }

    public async Task<bool> ExistsAsync(Guid groupId, Guid permissionId)
    {
        var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE GroupId = @GroupId AND PermissionId = @PermissionId";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { GroupId = groupId, PermissionId = permissionId });
        return count > 0;
    }

    public async Task<IEnumerable<GroupPermission>> GetByGroupIdAsync(Guid groupId)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE GroupId = @GroupId";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { GroupId = groupId });
        return results.Select(MapToEntity);
    }

    public async Task<IEnumerable<GroupPermission>> GetByPermissionIdAsync(Guid permissionId)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE PermissionId = @PermissionId";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { PermissionId = permissionId });
        return results.Select(MapToEntity);
    }

    public async Task DeleteByGroupAndPermissionAsync(Guid groupId, Guid permissionId)
    {
        var sql = $"DELETE FROM {_tableName} WHERE GroupId = @GroupId AND PermissionId = @PermissionId";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { GroupId = groupId, PermissionId = permissionId });
    }

    public async Task<int> CountByPermissionIdAsync(Guid permissionId)
    {
        var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE PermissionId = @PermissionId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new { PermissionId = permissionId });
    }

    protected override GroupPermission MapToEntity(dynamic result)
    {
        var groupPermission = new GroupPermission(
            (Guid)result.Id,
            (Guid)result.GroupId,
            (Guid)result.PermissionId,
            (Guid)result.AssignedByUserId
        );

        // Set timestamps using reflection
        var createdAtProperty = typeof(GroupPermission).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(groupPermission, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(GroupPermission).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        lastUpdatedAtProperty?.SetValue(groupPermission, result.LastUpdatedAt);

        return groupPermission;
    }
}
