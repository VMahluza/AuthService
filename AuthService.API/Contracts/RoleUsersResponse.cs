namespace AuthService.API.Contracts;

public record RoleUsersResponse(
    Guid RoleId,
    string RoleName,
    IEnumerable<UserSummaryResponse> Users
);

public record UserSummaryResponse(
    Guid Id,
    string UserName,
    string Email
);
