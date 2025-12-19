namespace AuthService.Application.Features.Groups.Queries.GetGroupUsers;

public record UserDto(
    Guid Id,
    string UserName,
    string Email
);

public record GetGroupUsersResult(
    Guid GroupId,
    string GroupName,
    IEnumerable<UserDto> Users
);
