using AuthService.API.Contracts.Common;

namespace AuthService.API.Contracts.Groups.GetGroup;

public record GroupUsersResponse(
    Guid GroupId,
    string GroupName,
    IEnumerable<UserSummaryResponse> Users
);
