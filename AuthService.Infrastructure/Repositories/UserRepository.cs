using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Domain.ValueObjects;
using AuthService.Domain.Enums;
using AuthService.Infrastructure.Database;
using Dapper;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(IAuthConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public override async Task AddAsync(User userEntity)
    {
        var sql = $@"
            INSERT INTO {_tableName} (Id, UserName, Email, PasswordHash, Status, FailedLoginAttempts, CreatedAt, LastUpdatedAt)
            VALUES (@Id, @UserName, @Email, @PasswordHash, @Status, @FailedLoginAttempts, @CreatedAt, @LastUpdatedAt)";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = userEntity.Id.ToString(),
            UserName = userEntity.UserName,
            Email = userEntity.Email.Value,
            PasswordHash = userEntity.PasswordHash.Value,
            Status = userEntity.Status.ToString(),
            FailedLoginAttempts = userEntity.FailedLoginAttempts,
            CreatedAt = userEntity.CreatedAt,
            LastUpdatedAt = userEntity.LastUpdatedAt
        });
    }

    public override async Task UpdateAsync(User userEntity)
    {
        var sql = $@"
            UPDATE {_tableName}
            SET UserName = @UserName, Email = @Email, PasswordHash = @PasswordHash, Status = @Status, 
                FailedLoginAttempts = @FailedLoginAttempts, LastUpdatedAt = @LastUpdatedAt
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = userEntity.Id.ToString(),
            UserName = userEntity.UserName,
            Email = userEntity.Email.Value,
            PasswordHash = userEntity.PasswordHash.Value,
            Status = userEntity.Status.ToString(),
            FailedLoginAttempts = userEntity.FailedLoginAttempts,
            LastUpdatedAt = DateTime.UtcNow
        });
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE Email = @Email";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { Email = email });
        return result != null ? MapToEntity(result) : null;
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE UserName = @UserName";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QuerySingleOrDefaultAsync<dynamic>(sql, new { UserName = username });
        return result != null ? MapToEntity(result) : null;
    }

    public async Task<bool> ExistsAsync(string email, string username)
    {
        var sql = $"SELECT COUNT(1) FROM {_tableName} WHERE Email = @Email OR UserName = @UserName";

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email, UserName = username });
        return count > 0;
    }

    protected override User MapToEntity(dynamic result)
    {
        var user = new User(
            (Guid)result.Id,  // Cast to Guid instead of parsing
            result.UserName,
            EmailAddress.Create(result.Email),
            PasswordHash.Create(result.PasswordHash),
            Enum.Parse<UserStatus>(result.Status),
            result.FailedLoginAttempts
        );

        // Set timestamps using reflection since setters are protected
        var createdAtProperty = typeof(User).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(user, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(User).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        lastUpdatedAtProperty?.SetValue(user, result.LastUpdatedAt);

        return user;
    }
}
