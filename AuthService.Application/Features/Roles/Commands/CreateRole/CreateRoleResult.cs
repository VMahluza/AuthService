namespace AuthService.Application.Features.Roles.Commands.CreateRole;

public record CreateRoleResult(
    Guid RoleId,
    string Name,
    string Description,
    DateTime CreatedAt
);
