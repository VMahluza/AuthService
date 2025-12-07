namespace AuthService.Application.Features.Roles.Commands.UpdateRole;

public record UpdateRoleResult(
    Guid RoleId,
    string Name,
    string Description,
    DateTime LastUpdatedAt
);
