namespace AuthService.API.Contracts;

public record UserGroupResponse(
    Guid UserId,
    string UserName,
    IEnumerable<GroupResponse> Groups
);
