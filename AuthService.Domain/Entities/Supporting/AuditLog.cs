using AuthService.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities.Supporting;

public class AuditLog : BaseEntity
{

    public Guid UserId { get; private set; }

    public string Action { get; private set; }
    public string ? Description { get; private set; }
    public string IpAddress { get; private set; }

    private AuditLog() { }

    public AuditLog(
        Guid id,
        Guid userId,
        string action,
        string? description,
        string ipAddress)
        : base(id)
    {
        UserId = userId;
        Action = action;
        Description = description;
        IpAddress = ipAddress;
    }

    // Factory method to create a new log entry easily
    public static AuditLog Create(Guid? userId, string action, string details, string ipAddress)
    {
        return new AuditLog(
            Guid.NewGuid(),
            userId ?? Guid.Empty,
            action,
            details,
            ipAddress
        );
    }

}