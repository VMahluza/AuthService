using AuthService.Domain.DTOs;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Reflection;
using System.Text;

namespace AuthService.Infrastructure.Repositories;

public class UserSessionRepository : BaseRepository<UserSession>, IUserSessionRepository
{
    public UserSessionRepository(IAuthConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public override async Task AddAsync(UserSession entity)
    {
        var entityProperties = typeof(UserSession).GetProperties();
        var sql = new StringBuilder($"INSERT INTO {_tableName} (");
        sql.Append(string.Join(", ", entityProperties.Select(p => p.Name)));
        sql.Append(") VALUES (");
        sql.Append(string.Join(", ", entityProperties.Select(p => $"@{p.Name}")));
        sql.Append(");");

        var parameters = new
        {
            entity.Id,
            entity.UserId,
            entity.JwtToken,
            entity.IssuedAt,
            entity.ExpiresAt,
            entity.RevokedAt,
            entity.CreatedAt,
            entity.LastUpdatedAt
        };
        
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql.ToString(), parameters);
    }

    public override async Task UpdateAsync(UserSession entity)
    {
        var sql = $@"
            UPDATE {_tableName}
            SET RevokedAt = @RevokedAt, LastUpdatedAt = @LastUpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql.ToString(), new
        {
            entity.Id,
            entity.RevokedAt,
            LastUpdatedAt = DateTime.UtcNow
        });
    }

    public async Task<UserSession?> GetActiveSessionByTokenAsync(string jwtToken)
    {
        var sql = $@"
            SELECT * FROM {_tableName} 
            WHERE JwtToken = @JwtToken 
              AND RevokedAt IS NULL 
              AND ExpiresAt > @Now";
              
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new 
        { 
            JwtToken = jwtToken,
            Now = DateTime.UtcNow
        });
        
        return result != null ? MapToEntity(result) : default;
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdPageAsync(Guid userId, int pageNumber, int pageSize)
    {
        var sql = $@"
            SELECT * FROM {_tableName} 
            WHERE UserId = @UserId 
              AND RevokedAt IS NULL 
              AND ExpiresAt > @Now
            ORDER BY IssuedAt DESC
            LIMIT @Offset, @PageSize";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new
        {
            UserId = userId.ToString(),
            Now = DateTime.UtcNow,
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        });
        
        return results.Select(MapToEntity);
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdAsync(Guid userId)
    {
        var sql = $@"
            SELECT * FROM {_tableName} 
            WHERE UserId = @UserId 
              AND RevokedAt IS NULL 
              AND ExpiresAt > @Now
            ORDER BY IssuedAt ASC";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new
        {
            UserId = userId.ToString(),
            Now = DateTime.UtcNow
        });
        
        return results.Select(MapToEntity);
    }

    public async Task<int> GetActiveSessionsCountAsync(Guid userId)
    {
        var sql = $@"
            SELECT COUNT(*) FROM {_tableName} 
            WHERE UserId = @UserId 
              AND RevokedAt IS NULL 
              AND ExpiresAt > @Now";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            UserId = userId.ToString(),
            Now = DateTime.UtcNow
        });
    }

    protected override UserSession MapToEntity(dynamic result)
    {
        var userSession = new UserSession(
            (Guid)result.Id,
            (Guid)result.UserId,
            new AuthenticationResult(
                result.JwtToken,
                result.JwtToken,
                Guid.NewGuid().ToString(), 
                (DateTime)result.ExpiresAt
            ),
     
            (DateTime)result.ExpiresAt
        );

        var issuedAtProperty = typeof(UserSession).GetProperty("IssuedAt");
        issuedAtProperty?.SetValue(userSession, result.IssuedAt);

        var revokedAtProperty = typeof(UserSession).GetProperty("RevokedAt");
        if (result.RevokedAt != null)
        {
            revokedAtProperty?.SetValue(userSession, (DateTime?)result.RevokedAt);
        }

        var createdAtProperty = typeof(UserSession).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(userSession, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(UserSession).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        lastUpdatedAtProperty?.SetValue(userSession, result.LastUpdatedAt);

        return userSession;
    }
}
