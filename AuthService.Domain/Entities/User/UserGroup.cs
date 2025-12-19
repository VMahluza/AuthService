using AuthService.Domain.Entities.Common;
using System;

namespace AuthService.Domain.Entities.User;

/// <summary>
/// Join table for User-Group many-to-many relationship
/// </summary>
public class UserGroup : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid GroupId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public Guid? AssignedBy { get; private set; } // Admin who assigned the user to group

    /// <summary>
    /// For Dapper reconstitution
    /// </summary>
    private UserGroup() { }

    public UserGroup(Guid id, Guid userId, Guid groupId, Guid? assignedBy = null) : base(id)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (groupId == Guid.Empty)
            throw new ArgumentException("GroupId cannot be empty.", nameof(groupId));

        UserId = userId;
        GroupId = groupId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }

    public static UserGroup Create(Guid userId, Guid groupId, Guid? assignedBy = null)
    {
        return new UserGroup(Guid.NewGuid(), userId, groupId, assignedBy);
    }
}
