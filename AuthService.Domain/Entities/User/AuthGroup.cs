using AuthService.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities.User;

/// <summary>
/// Represents a Group for organizing users and permissions.
/// Groups are the primary way to bundle permissions and users.
/// </summary>
public class AuthGroup : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private AuthGroup() { }

    public AuthGroup(Guid id, string name, string description) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name cannot be empty.", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
    }

    public static AuthGroup Create(string name, string description)
    {
        return new AuthGroup(Guid.NewGuid(), name, description);
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name cannot be empty.", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
        LastUpdatedAt = DateTime.UtcNow;
    }
}

