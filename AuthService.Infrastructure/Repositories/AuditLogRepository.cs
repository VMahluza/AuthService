using AuthService.Domain.Entities.Supporting;
using AuthService.Domain.Entities.User;
using AuthService.Domain.Interfaces.Repositories;
using AuthService.Infrastructure.Database;
using Dapper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AuthService.Infrastructure.Repositories;

public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
{

    public AuditLogRepository(IAuthConnectionFactory connectionFactory) : base(connectionFactory)
    {
        
    }

    public override async Task AddAsync(AuditLog auditLog)
    { 
         
        var entityProperties = typeof(AuditLog).GetProperties();
        var sql = new StringBuilder($"INSERT INTO {_tableName} (");
        sql.Append(string.Join(", ", entityProperties.Select(p => p.Name)));
        sql.Append(") VALUES (");
        sql.Append(string.Join(", ", entityProperties.Select(p => $"@{p.Name}")));
        sql.Append(");");

        var parameters = new
        {

            Id = auditLog.Id.ToString(),
            UserId = auditLog.UserId.ToString(),
            Action = auditLog.Action,
            Description = auditLog.Description,
            IpAddress = auditLog.IpAddress,
            CreatedAt = auditLog.CreatedAt,
            LastUpdatedAt = auditLog.LastUpdatedAt

        };
        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql.ToString(), parameters);
        //await base.AddAsync(auditLog);
    }

    protected override AuditLog MapToEntity(dynamic result)
    {
        var auditLog = new AuditLog(
            (Guid)result.Id,
            (Guid)result.UserId,
            result.Action,
            result.Description,
            result.IpAddress
        );

        var createdAtProperty = typeof(AuditLog).BaseType.GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        createdAtProperty?.SetValue(auditLog, result.CreatedAt);

        var lastUpdatedAtProperty = typeof(AuditLog).BaseType.GetProperty("LastUpdatedAt", BindingFlags.NonPublic | BindingFlags.Instance);
        lastUpdatedAtProperty?.SetValue(auditLog, result.LastUpdatedAt);

        return auditLog;
    }
}