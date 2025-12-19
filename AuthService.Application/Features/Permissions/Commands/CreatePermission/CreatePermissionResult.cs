namespace AuthService.Application.Features.Permissions.Commands.CreatePermission;

public record CreatePermissionResult(
    Guid PermissionId,
    string Key,
    string Name,
    string Description,
    DateTime CreatedAt
);
