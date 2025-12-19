namespace AuthService.Application.Features.Roles.Queries.GetRoles;

public record RoleDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);

public record GetRolesResult(
    IEnumerable<RoleDto> Roles,
    int TotalCount,
    int PageNumber,
    int PageSize
);
