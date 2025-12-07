using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Reflection;

namespace AuthService.Infrastructure.Repositories;

public class GroupRepository : BaseRepository<AuthGroup>, IGroupRepository
{
    public GroupRepository(IAuthConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public override async Task AddAsync(AuthGroup entity)
    {
        var sql = $@"
            INSERT INTO {_tableName} (Id, Name, Description, CreatedAt, LastUpdatedAt)
            VALUES (@Id, @Name, @Description, @CreatedAt, @LastUpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            LastUpdatedAt = entity.LastUpdatedAt
        });
    }

    public override async Task UpdateAsync(AuthGroup entity)
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

    public async Task<AuthGroup?> GetByNameAsync(string name)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE Name = @Name";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Name = name });
        return result != null ? MapToEntity(result) : null;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE Name = @Name";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Name = name });
        return count > 0;
    }

    public async Task<IEnumerable<AuthGroup>> GetAllAsync(int pageNumber, int pageSize)
    {
        var sql = $@"
            SELECT * FROM {_tableName} 
            ORDER BY Name 
            LIMIT @Offset, @PageSize";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new
        {
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        });

        return results.Select(MapToEntity);
    }

    public async Task<int> GetUserCountAsync(Guid groupId)
    {
        var sql = "SELECT COUNT(*) FROM UserGroups WHERE GroupId = @GroupId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new { GroupId = groupId });
    }

    protected override AuthGroup MapToEntity(dynamic result)
    {
        var group = new AuthGroup(
            (Guid)result.Id,
            result.Name,
            result.Description
        );

        // Set timestamps using reflection
        var createdAtProperty = typeof(AuthGroup).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(group, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(AuthGroup).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        if (result.LastUpdatedAt != null)
        {
            lastUpdatedAtProperty?.SetValue(group, (DateTime?)result.LastUpdatedAt);
        }

        return group;
    }
}
