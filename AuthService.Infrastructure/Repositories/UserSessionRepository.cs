using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AuthService.Infrastructure.Repositories;
public class UserSessionRepository :BaseRepository<UserSession>,  IUserSessionRepository
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

    public async Task<UserSession?> GetActiveSessionByTokenAsync(string jwtToken)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE JwtToken = @JwtToken";
        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { JwtToken = jwtToken });
        return result != null ? MapToEntity(result) : default;
    }

    public async Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdPageAsync(Guid userId, int pageNumber, int pageSize)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE UserId = @UserId LIMIT @Offset, @PageSize";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<dynamic>(sql, new
        {
            UserId = userId.ToString(),
            Offset = (pageNumber - 1) * pageSize,
            PageSize = pageSize
        });
        return results.Select(MapToEntity);
    }



    protected override UserSession MapToEntity(dynamic result)
    {
        UserSession userSession = new UserSession(

            (Guid)result.Id,
            (Guid)result.UserId,
            (string)result.JwtToken,
            (DateTime)result.ExpiresAt
        );

        var createdAtProperty = typeof(UserSession).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(userSession, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(UserSession).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        lastUpdatedAtProperty?.SetValue(userSession, result.LastUpdatedAt);

        return userSession;

    }
}
