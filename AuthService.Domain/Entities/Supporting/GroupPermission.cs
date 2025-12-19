using AuthService.Domain.Entities.Common;
using System;

namespace AuthService.Domain.Entities.Supporting;

/// <summary>
/// Represents the assignment of a Permission to a Group.
/// This is the join entity for the many-to-many relationship between Groups and Permissions.
/// </summary>
public class GroupPermission : BaseEntity
{
    public Guid GroupId { get; private set; }
    public Guid PermissionId { get; private set; }
    public Guid AssignedByUserId { get; private set; }

    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private GroupPermission() { }

    public GroupPermission(Guid id, Guid groupId, Guid permissionId, Guid assignedByUserId) : base(id)
    {
        if (groupId == Guid.Empty)
            throw new ArgumentException("GroupId cannot be empty.", nameof(groupId));
        
        if (permissionId == Guid.Empty)
            throw new ArgumentException("PermissionId cannot be empty.", nameof(permissionId));
        
        if (assignedByUserId == Guid.Empty)
            throw new ArgumentException("AssignedByUserId cannot be empty.", nameof(assignedByUserId));

        GroupId = groupId;
        PermissionId = permissionId;
        AssignedByUserId = assignedByUserId;
    }

    public static GroupPermission Create(Guid groupId, Guid permissionId, Guid assignedByUserId)
    {
        return new GroupPermission(Guid.NewGuid(), groupId, permissionId, assignedByUserId);
    }
}
