using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Reflection;

namespace AuthService.Infrastructure.Repositories;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(IAuthConnectionFactory connectionFactory) : base(connectionFactory)
    {
        _tableName = "Permissions";
    }

    public override async Task AddAsync(Permission entity)
    {
        var sql = $@"
            INSERT INTO {_tableName} (Id, `Key`, Name, Description, CreatedAt, LastUpdatedAt)
            VALUES (@Id, @Key, @Name, @Description, @CreatedAt, @LastUpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            Key = entity.Key,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            LastUpdatedAt = entity.LastUpdatedAt
        });
    }

    public override async Task UpdateAsync(Permission entity)
    {
        var sql = $@"
            UPDATE {_tableName}
            SET Name = @Name, Description = @Description, LastUpdatedAt = @LastUpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            LastUpdatedAt = DateTime.UtcNow
        });
    }

    public async Task<Permission?> GetByKeyAsync(string key)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE `Key` = @Key";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Key = key.ToUpperInvariant() });
        return result != null ? MapToEntity(result) : null;
    }

    public async Task<bool> ExistsByKeyAsync(string key)
    {
        var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE `Key` = @Key";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Key = key.ToUpperInvariant() });
        return count > 0;
    }

    public async Task<IEnumerable<Permission>> GetAllAsync()
    {
        var sql = $"SELECT * FROM {_tableName} ORDER BY Name";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql);
        return results.Select(MapToEntity);
    }

    public async Task<IEnumerable<Permission>> GetByGroupIdAsync(Guid groupId)
    {
        var sql = $@"
            SELECT p.* FROM {_tableName} p
            INNER JOIN GroupPermissions gp ON p.Id = gp.PermissionId
            WHERE gp.GroupId = @GroupId
            ORDER BY p.Name";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { GroupId = groupId });
        return results.Select(MapToEntity);
    }

    public async Task<IEnumerable<Permission>> GetEffectivePermissionsByUserIdAsync(Guid userId)
    {
        var sql = $@"
            SELECT DISTINCT p.* FROM {_tableName} p
            INNER JOIN GroupPermissions gp ON p.Id = gp.PermissionId
            INNER JOIN UserGroups ug ON gp.GroupId = ug.GroupId
            WHERE ug.UserId = @UserId
            ORDER BY p.Name";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { UserId = userId });
        return results.Select(MapToEntity);
    }

    protected override Permission MapToEntity(dynamic result)
    {
        var permission = new Permission(
            (Guid)result.Id,
            result.Key,
            result.Name,
            result.Description
        );

        // Set timestamps using reflection
        var createdAtProperty = typeof(Permission).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(permission, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(Permission).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        lastUpdatedAtProperty?.SetValue(permission, result.LastUpdatedAt);

        return permission;
    }
}
