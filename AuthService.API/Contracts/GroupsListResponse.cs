namespace AuthService.API.Contracts;

public record GroupsListResponse(
    IEnumerable<GroupResponse> Groups,
    int TotalCount,
    int PageNumber,
    int PageSize
);
