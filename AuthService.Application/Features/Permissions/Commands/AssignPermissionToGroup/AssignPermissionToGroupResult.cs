namespace AuthService.Application.Features.Permissions.Commands.AssignPermissionToGroup;

public record AssignPermissionToGroupResult(
    bool Success,
    string Message,
    Guid GroupId,
    Guid PermissionId,
    string PermissionKey
);
