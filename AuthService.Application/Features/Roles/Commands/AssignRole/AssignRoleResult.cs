namespace AuthService.Application.Features.Roles.Commands.AssignRole;

public record AssignRoleResult(
    bool Success,
    string Message,
    Guid UserId,
    Guid RoleId,
    string RoleName
);
