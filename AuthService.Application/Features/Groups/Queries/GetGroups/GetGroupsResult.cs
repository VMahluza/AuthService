namespace AuthService.Application.Features.Groups.Queries.GetGroups;

public record GroupDto(
    Guid Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt
);

public record GetGroupsResult(
    IEnumerable<GroupDto> Groups,
    int TotalCount,
    int PageNumber,
    int PageSize
);
