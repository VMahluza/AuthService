namespace AuthService.API.Contracts;

public record UserRoleResponse(
    Guid UserId,
    string UserName,
    IEnumerable<RoleResponse> Roles
);
