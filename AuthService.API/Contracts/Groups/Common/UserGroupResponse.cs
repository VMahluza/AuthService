using AuthService.API.Contracts.Groups.GetGroup;

namespace AuthService.API.Contracts.Groups.Common;

public record UserGroupResponse(
    Guid UserId,
    string UserName,
    IEnumerable<GroupResponse> Groups
);
