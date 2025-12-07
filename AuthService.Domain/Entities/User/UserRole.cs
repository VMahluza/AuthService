using System;

namespace AuthService.Domain.Entities.User;

/// <summary>
/// Join table for User-Role many-to-many relationship
/// </summary>
public class UserRole
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; } // Admin who assigned the role

    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private UserRole() { }

    public UserRole(Guid userId, Guid roleId, Guid? assignedBy = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (roleId == Guid.Empty)
            throw new ArgumentException("RoleId cannot be empty.", nameof(roleId));

        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }
}
