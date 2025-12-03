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

    public Task<UserSession?> GetActiveSessionByTokenAsync(string jwtToken)
    {

        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserSession>> GetActiveSessionsByUserIdPageAsync(Guid userId, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
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
