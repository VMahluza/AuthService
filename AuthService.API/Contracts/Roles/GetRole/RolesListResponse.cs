namespace AuthService.API.Contracts.Roles.GetRole;

public record RolesListResponse(
    IEnumerable<RoleResponse> Roles,
    int TotalCount,
    int PageNumber,
    int PageSize
);
