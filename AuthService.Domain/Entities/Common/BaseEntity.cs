using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? LastUpdatedAt { get; protected set; }

    // Parameterless constructor for Dapper rehydration
    protected BaseEntity() { }
    protected BaseEntity(Guid id)
    {

        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        Id = id;
        CreatedAt = DateTime.UtcNow;
        LastUpdatedAt = null;
    }
}