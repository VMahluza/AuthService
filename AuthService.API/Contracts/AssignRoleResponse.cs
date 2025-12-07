namespace AuthService.API.Contracts;

public record AssignRoleResponse(
    bool Success,
    string Message,
    Guid UserId,
    Guid RoleId,
    string RoleName
);
