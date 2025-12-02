using AuthService.Domain.Entities.Common;
using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Data;
using System.Reflection;
using System.Text;

namespace AuthService.Infrastructure.Repositories;

public class EmailVerificationTokenRepository : BaseRepository<EmailVerificationToken>, IEmailVerificationTokenRepository
{
    public EmailVerificationTokenRepository(IAuthConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task AddAsync(EmailVerificationToken token, IDbTransaction? transaction = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO EmailVerificationTokens (Id, UserId, Token, ExpiresAt, UsedAt, CreatedAt, LastUpdatedAt)");
        sql.AppendLine("VALUES (@Id, @UserId, @Token, @ExpiresAt, @UsedAt, @CreatedAt, @LastUpdatedAt);");

        var parameters = new
        {
            token.Id,
            token.UserId,
            token.Token,
            token.ExpiresAt,
            token.UsedAt,
            token.CreatedAt,
            token.LastUpdatedAt
        };

        await connection.ExecuteAsync(sql.ToString(), parameters, transaction);
    }

    public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE Token = @Token";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Token = token });
        return result != null ? MapToEntity(result) : null;
    }

    protected override EmailVerificationToken MapToEntity(dynamic result)
    {
        var token = new EmailVerificationToken(
            (Guid)result.Id,
            (Guid)result.UserId,
            result.Token,
            result.ExpiresAt,
            result.UsedAt
        );
        var createdAtProp = typeof(BaseEntity).GetProperty("CreatedAt",
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance);

        createdAtProp?.SetValue(token, result.CreatedAt);

        var lastUpdatedProp = typeof(BaseEntity).GetProperty("LastUpdatedAt",
            BindingFlags.NonPublic |
            BindingFlags.Public |
            BindingFlags.Instance);

        lastUpdatedProp?.SetValue(token, result.LastUpdatedAt);

        return token;
    }
}