namespace AuthService.API.Contracts;

public record RolesListResponse(
    IEnumerable<RoleResponse> Roles,
    int TotalCount,
    int PageNumber,
    int PageSize
);
