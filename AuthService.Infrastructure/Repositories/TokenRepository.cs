using AuthService.Domain.Entities.Common;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Reflection;

namespace AuthService.Infrastructure.Repositories;

public class TokenRepository<T> : BaseRepository<T>, ITokenRepository<T> 
    where T : ExpiringToken
{
    private readonly TokenType _tokenType;

    public TokenRepository(IAuthConnectionFactory connectionFactory, TokenType tokenType) 
        : base(connectionFactory)
    {
        _tokenType = tokenType;
        _tableName = "Tokens"; // Override base convention
    }

    public override async Task AddAsync(T entity)
    {
        var sql = @"
            INSERT INTO Tokens (Id, UserId, Token, TokenType, ExpiresAt, UsedAt, CreatedAt, LastUpdatedAt)
            VALUES (@Id, @UserId, @Token, @TokenType, @ExpiresAt, @UsedAt, @CreatedAt, @LastUpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Token = entity.Token,
            TokenType = entity.TokenType.ToString(),
            ExpiresAt = entity.ExpiresAt,
            UsedAt = entity.UsedAt,
            CreatedAt = entity.CreatedAt,
            LastUpdatedAt = entity.LastUpdatedAt
        });
    }

    public override async Task UpdateAsync(T entity)
    {
        var sql = @"
            UPDATE Tokens 
            SET UsedAt = @UsedAt, LastUpdatedAt = @LastUpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = entity.Id,
            UsedAt = entity.UsedAt,
            LastUpdatedAt = DateTime.UtcNow
        });
    }

    public async Task<T?> GetByTokenAsync(string token)
    {
        var sql = "SELECT * FROM Tokens WHERE Token = @Token AND TokenType = @TokenType";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<T>(
            sql, 
            new { Token = token, TokenType = _tokenType.ToString() }
        );
        
        return result;
    }

    public async Task<IEnumerable<T>> GetByUserIdAsync(Guid userId)
    {
        var sql = "SELECT * FROM Tokens WHERE UserId = @UserId AND TokenType = @TokenType ORDER BY CreatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<T>(
            sql,
            new { UserId = userId, TokenType = _tokenType.ToString() }
        );

        return results;
    }

    public async Task InvalidateAllForUserAsync(Guid userId)
    {
        var sql = @"
            UPDATE Tokens 
            SET UsedAt = @UsedAt, LastUpdatedAt = @LastUpdatedAt
            WHERE UserId = @UserId AND TokenType = @TokenType AND UsedAt IS NULL";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            TokenType = _tokenType.ToString(),
            UsedAt = DateTime.UtcNow,
            LastUpdatedAt = DateTime.UtcNow
        });
    }

    protected override T MapToEntity(dynamic result)
    {
        // Not used - Dapper handles mapping directly using the parameterless constructor
        throw new NotImplementedException("This method is not used. Dapper maps directly to type T.");
    }
}