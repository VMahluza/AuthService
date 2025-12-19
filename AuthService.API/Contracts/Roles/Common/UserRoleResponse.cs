using AuthService.API.Contracts.Roles.GetRole;

namespace AuthService.API.Contracts.Roles.Common;

public record UserRoleResponse(
    Guid UserId,
    string UserName,
    IEnumerable<RoleResponse> Roles
);
