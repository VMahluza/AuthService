namespace AuthService.Application.Features.Roles.Queries.GetRoleUsers;

public record UserDto(
    Guid Id,
    string UserName,
    string Email
);

public record GetRoleUsersResult(
    Guid RoleId,
    string RoleName,
    IEnumerable<UserDto> Users
);
