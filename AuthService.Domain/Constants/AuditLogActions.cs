using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Constants;
public static class AuditLogActions
{
    // User Lifecycle
    public const string UserRegistered = "USER_REGISTERED";
    public const string EmailVerified = "EMAIL_VERIFIED";
    public const string AccountLocked = "ACCOUNT_LOCKED";
    public const string AccountUnlocked = "ACCOUNT_UNLOCKED";

    // Authentication
    public const string LoginSuccess = "LOGIN_SUCCESS";
    public const string LoginFailure = "LOGIN_FAILURE";
    public const string Logout = "USER_LOGOUT";

    // Password Management
    public const string PasswordResetRequested = "PASSWORD_RESET_REQUESTED";
    public const string PasswordResetCompleted = "PASSWORD_RESET_COMPLETED";
    public const string PasswordChanged = "PASSWORD_CHANGED";

    // Role Management
    public const string RoleCreated = "ROLE_CREATED";
    public const string RoleUpdated = "ROLE_UPDATED";
    public const string RoleDeleted = "ROLE_DELETED";
    public const string RoleAssignedToUser = "ROLE_ASSIGNED_TO_USER";
    public const string RoleRemovedFromUser = "ROLE_REMOVED_FROM_USER";

    // Group Management
    public const string GroupCreated = "GROUP_CREATED";
    public const string GroupUpdated = "GROUP_UPDATED";
    public const string GroupDeleted = "GROUP_DELETED";
    public const string UserAddedToGroup = "USER_ADDED_TO_GROUP";
    public const string UserRemovedFromGroup = "USER_REMOVED_FROM_GROUP";
}