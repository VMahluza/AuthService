namespace AuthService.API.Contracts.Groups.GetGroup;

public record GroupsListResponse(
    IEnumerable<GroupResponse> Groups,
    int TotalCount,
    int PageNumber,
    int PageSize
);
