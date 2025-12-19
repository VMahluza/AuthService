namespace AuthService.API.Contracts.Roles.AssignRole;

public record AssignRoleResponse(
    bool Success,
    string Message,
    Guid UserId,
    Guid RoleId,
    string RoleName
);
