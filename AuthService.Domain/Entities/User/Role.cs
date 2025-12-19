using AuthService.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities.User;

/// <summary>
/// Represents a Role for classifying users (e.g., Admin, Manager, Customer).
/// Roles are typically tied to job function or high-level access categories.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private Role() { }

    public Role(Guid id, string name, string description) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
    }

    public static Role Create(string name, string description)
    {
        return new Role(Guid.NewGuid(), name, description);
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty.", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
        LastUpdatedAt = DateTime.UtcNow;
    }
}

