namespace AuthService.Application.Features.Roles.Queries.GetUserRoles;

public record RoleDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);

public record GetUserRolesResult(
    Guid UserId,
    string UserName,
    IEnumerable<RoleDto> Roles
);
