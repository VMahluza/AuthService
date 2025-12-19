using AuthService.Domain.Entities.Common;
using System;

namespace AuthService.Domain.Entities.User;

/// <summary>
/// Represents a Permission as a discrete access right.
/// Permissions correspond to fine-grained actions in the application.
/// Example: "RESET_PASSWORD", "DELETE_USER", "VIEW_AUDIT_LOGS"
/// </summary>
public class Permission : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Key { get; private set; }

    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private Permission() { }

    public Permission(Guid id, string key, string name, string description) : base(id)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Permission key cannot be empty.", nameof(key));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty.", nameof(name));

        Key = key.ToUpperInvariant();
        Name = name;
        Description = description ?? string.Empty;
    }

    public static Permission Create(string key, string name, string description)
    {
        return new Permission(Guid.NewGuid(), key, name, description);
    }

    public void UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Permission name cannot be empty.", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
        LastUpdatedAt = DateTime.UtcNow;
    }
}
