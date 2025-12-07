namespace AuthService.Application.Features.Groups.Queries.GetUserGroups;

public record GroupDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);

public record GetUserGroupsResult(
    Guid UserId,
    string UserName,
    IEnumerable<GroupDto> Groups
);
