using AuthService.Domain.Entities.Common;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;

namespace AuthService.Infrastructure.Repositories;

public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly IAuthConnectionFactory _connectionFactory;
    protected virtual string _tableName { get; set; }

    protected BaseRepository(IAuthConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _tableName = typeof(T).Name + "s"; // Convention: e.g., User -> Users
    }

    public virtual async Task AddAsync(T entity)
    {
        // This is a generic implementation; subclasses may override for specific field handling
        var sql = $@"
            INSERT INTO {_tableName} (Id, CreatedAt, LastUpdatedAt)
            VALUES (@Id, @CreatedAt, @LastUpdatedAt)";


        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id.ToString(),
            CreatedAt = entity.CreatedAt,
            LastUpdatedAt = entity.LastUpdatedAt
        });
    }

    public virtual async Task UpdateAsync(T entity)
    {
        var sql = $@"
            UPDATE {_tableName}
            SET LastUpdatedAt = @LastUpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id.ToString(),
            LastUpdatedAt = DateTime.UtcNow
        });
    }

    public virtual async Task DeleteAsync(T entity)
    {
        var sql = $"DELETE FROM {_tableName} WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new { Id = entity.Id.ToString() });
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Id = id.ToString() });
        return result != null ? MapToEntity(result) : default;
    }

    public virtual async Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var sql = $"SELECT * FROM {_tableName} ORDER BY CreatedAt DESC LIMIT @Offset, @PageSize";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new { Offset = (pageNumber - 1) * pageSize, PageSize = pageSize });
        return results.Select(MapToEntity);
    }

    // Abstract method for entity-specific mapping
    protected abstract T MapToEntity(dynamic result);
}
