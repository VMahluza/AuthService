using AuthService.API.Contracts.Common;

namespace AuthService.API.Contracts.Roles.GetRole;

public record RoleUsersResponse(
    Guid RoleId,
    string RoleName,
    IEnumerable<UserSummaryResponse> Users
);
