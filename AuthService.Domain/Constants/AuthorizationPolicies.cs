namespace AuthService.Domain.Constants;

/// <summary>
/// Authorization policy names used throughout the application
/// </summary>
public static class AuthorizationPolicies
{
    // Role-based policies
    public const string RequireAdminRole = "RequireAdminRole";
    public const string RequireManagerRole = "RequireManagerRole";
    public const string RequireUserRole = "RequireUserRole";

    // Permission-based policies (for future use)
    public const string CanManageRoles = "CanManageRoles";
    public const string CanManageGroups = "CanManageGroups";
    public const string CanManageUsers = "CanManageUsers";
    public const string CanViewAuditLogs = "CanViewAuditLogs";
}
